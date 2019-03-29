using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HDFSwindow.HDFSCore
{
    /// <summary>
    /// HDFS 功能类
    /// </summary>
    public static class HdfsHelper
    {
        private static Dictionary<string, string> _ipTable = new Dictionary<string, string>();

        /// <summary>
        /// WebHDFS 服务地址和端口
        /// </summary>
        public static string WebHdfsHost = "localhost:50070";
        
        private static string BuildUrl(string hdfsPath, string api, string args = "")
        {
            string host = WebHdfsHost;
            if (!string.IsNullOrWhiteSpace(hdfsPath) && 
                hdfsPath.StartsWith("hdfs://", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = ParseHdfsIPAndPath(hdfsPath);
                if (!string.IsNullOrWhiteSpace(parts[0]))
                    host = string.Format("{0}:{1}", parts[0], 50070);
                hdfsPath = parts[1];
            }
            if (string.IsNullOrWhiteSpace(hdfsPath) || !hdfsPath.StartsWith("/"))
                throw new ArgumentException("Invalid path: " + hdfsPath);
            
            if (host.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                host = host.Replace("http://", "").Replace("HTTP://", "");
            string url = string.Format("http://{0}{1}{2}?op={3}",
                host,
                WebHdfsAPI.API_PREFIX,
                hdfsPath,
                api);
            if (!string.IsNullOrWhiteSpace(args))
                url += "&" + args;
            return url;
        }

        private static string[] ParseHdfsIPAndPath(string fullHdfsPath)
        {
            int ipPos = fullHdfsPath.IndexOf("//") + 2;
            string masterIP = fullHdfsPath.Substring(ipPos, fullHdfsPath.IndexOf(':', ipPos) - ipPos);
            string path = fullHdfsPath.Substring(fullHdfsPath.IndexOf('/', ipPos));
            return new string[] { masterIP, path };
        }

        /// <summary>
        /// 设置HDFS主节点IP地址
        /// </summary>
        /// <param name="nmAddress">NameNode的IP地址</param>
        public static void SetHDFSNameNode(string nmAddress)
        {
            WebHdfsHost = nmAddress + ":50070";

            var dict = GetDataNodes();
            foreach (var item in dict)
            {
                _ipTable[item.Key] = item.Value;
            }
        }

        public static Dictionary<string, string> GetDataNodes()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                string url = "HTTP://" + WebHdfsHost + WebHdfsAPI.HDFS_LIVENODES;
                string jsonstr = HttpClientHelperV45.Get(url);
                if (string.IsNullOrWhiteSpace(jsonstr))
                    return null;
                var jObj = JsonConvert.DeserializeObject(jsonstr);
                if (null == jObj)
                    throw new Exception("HDFS LiveNodes Json解析错误");
                var liveNodes = (jObj as JObject)["beans"][0]["LiveNodes"] as JValue;

                var jobjNodes = JsonConvert.DeserializeObject(liveNodes.ToString()) as JObject;
                foreach (var jprop in jobjNodes.Properties())
                {
                    string hname = jprop.Name.Split(':')[0];
                    string addr = (jprop.Value as JObject)["infoAddr"].ToString();
                    string ip = addr.Split(':')[0];
                    dict.Add(hname, ip);
                }

                return dict;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
            
        }

        /// <summary>
        /// 创建目录（自动创建上级目录）
        /// </summary>
        /// <param name="path">HDFS路径</param>
        /// <returns></returns>
        public static bool MkDir(string path)
        {
            try
            {
                string url = BuildUrl(path, WebHdfsAPI.MKDIR, "user.name=hadoop");
                HttpClientHelperV45.Put(url);
                return true;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 删除路径
        /// </summary>
        /// <param name="path">文件或路径名</param>
        /// <param name="recursively">是否删除所有下级子目录和文件</param>
        /// <returns></returns>
        public static bool RmDir(string path, bool recursively = true)
        {
            try
            {
                string url = BuildUrl(path, WebHdfsAPI.RM, "recursive=true");
                HttpClientHelperV45.Delete(url);
                return true;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 删除文件或目录
        /// </summary>
        /// <param name="file">文件或路径名</param>
        /// <returns></returns>
        public static bool Rm(string file)
        {
            return RmDir(file, true);
        }

        /// <summary>
        /// 列举当前目录内容
        /// </summary>
        /// <param name="path">HDFS路径</param>
        /// <returns></returns>
        public static List<HdfsFileInfo> LsDir(string path)
        {
            try
            {
                string url = BuildUrl(path, WebHdfsAPI.LIST);
                string result = HttpClientHelperV45.Get(url);
                return HdfsFileInfo.ParseJsonArray(result);
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取文件或路径的详细信息
        /// </summary>
        /// <param name="file">文件或路径名</param>
        /// <returns></returns>
        public static HdfsFileInfo GetStatus(string file)
        {
            try
            {
                string url = BuildUrl(file, WebHdfsAPI.FILESTATUS);
                string result = HttpClientHelperV45.Get(url);
                return HdfsFileInfo.FromJson(result);
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 下载HDFS文件
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns></returns>
        public static byte[] GetFile(string file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file))
                    throw new ArgumentNullException("file");
                if (null == GetStatus(file))
                    return Encoding.UTF8.GetBytes("File Not Exist.");
                string url = BuildUrl(file, WebHdfsAPI.OPEN, "user.name=atlas&noredirect=false");
                HttpResult res = HttpClientHelperV45.Request(url, new TimeSpan(1, 0, 0));
                if (200 == res.Code)
                    return res.Content;
                else
                    throw new Exception(res.ErrorLog);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                if (ex is WebException)
                    err = HttpClientHelperV45.ParseHdfsWebException(ex as WebException);
                DebugHelper.OutLog("获取HDFS文件失败：" + err);
                DebugHelper.Error(ex, err);
                return null;
            }
        }

        public static HttpResult ReadFilePart(string file, long offset, long size)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file))
                    throw new ArgumentNullException("file");
                if (null == GetStatus(file))
                    return new HttpResult()
                    {
                        Code = -1,
                        ErrorLog = "File Not Exist."
                    };

                string args = string.Format("&offset={0}&length={1}", offset, size);
                string url = BuildUrl(file, WebHdfsAPI.OPEN, "user.name=hadoop&noredirect=false" + args);
                return HttpClientHelperV45.Request(url, new TimeSpan(0, 0, 6));                
            }
            catch (Exception ex)
            {
                DebugHelper.OutLog("获取HDFS文件失败：" + ex.Message);
                DebugHelper.Error(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 上传文件（自动生成上级目录）
        /// </summary>
        /// <param name="srcFile">本地文件路径</param>
        /// <param name="hdfsPath">HDFS目标路径</param>
        /// <param name="errlog">日志信息</param>
        /// <returns></returns>
        public static bool UploadFile(string srcFile, string hdfsPath, out string errlog)
        {
            try
            {
                // 确保目录存在
                MkDir(hdfsPath);

                // 自动拼接文件名
                string fileName = new FileInfo(srcFile).Name;
                if (!hdfsPath.Contains(fileName))
                    hdfsPath += "/" + fileName;

                // Step 1. redirect to datanode
                // CAUSE: avoid sending file data before the server redirection.  and Jetty 6 server didn't implement "Expect: 100-continue" protocol.
                string url = BuildUrl(hdfsPath, WebHdfsAPI.UPLOAD,
                    "user.name=hadoop&overwrite=true&noredirect=true");
                /*string result = HttpClientHelperV45.PutResponse(url);
                if (!string.IsNullOrWhiteSpace(result) && result.Contains("Location"))
                {
                    url = result.Substring(result.IndexOf("http:", StringComparison.OrdinalIgnoreCase));
                    url = url.Remove(url.IndexOf("\""));
                }
                 **/
                //Hadoop 2.7.1 服务端总是会重定向（307 TEMPORARY_REDIRECT），noredirect=true参数根本不管用
                string redirUrl = HttpClientHelperV45.GetRedirectedURL(url, HttpMethod.PUT);
                if (!string.IsNullOrWhiteSpace(redirUrl))
                {
                    url = TranslateHostname(redirUrl);
                    DebugHelper.OutLog("Redirect to " + url);
                }

                // Step 2. upload file to datanode
                using (FileStream fs = new FileStream(srcFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string response = HttpClientHelperV45.HttpUpload(url, fs, HttpMethod.PUT);
                    fs.Close();
                }
                errlog = "succeed";
                return true;
            }
            catch (System.Exception ex)
            {
                string err = ex.Message;
                if (ex is WebException)
                    err = HttpClientHelperV45.ParseHdfsWebException(ex as WebException);
                DebugHelper.OutLog("上传文件到HDFS失败：" + err);
                DebugHelper.Error(ex, err);
                errlog = err;
                return false;
            }
        }

        public static bool UploadStream(Stream ins, long offset, long length, string hdfsDir, string fileName)
        {
            try
            {
                // 确保目录存在
                MkDir(hdfsDir);
                // 拼接文件名
                string hdfsFile = hdfsDir + "/" + fileName;
                if (hdfsDir.EndsWith("/"))
                    hdfsFile = hdfsDir + fileName;

                // Step 1. redirect to datanode
                string url = BuildUrl(hdfsFile, WebHdfsAPI.UPLOAD,
                    "user.name=hadoop&overwrite=true&noredirect=true");
                string redirUrl = HttpClientHelperV45.GetRedirectedURL(url, HttpMethod.PUT);
                if (!string.IsNullOrWhiteSpace(redirUrl))
                {
                    url = TranslateHostname(redirUrl);
                    DebugHelper.OutLog("Redirect to " + url);
                }
                // Step 2. upload file to datanode
                string response = HttpClientHelperV45.HttpUpload(url, ins, offset, length, HttpMethod.PUT);         
                return null != response;
            }
            catch (System.Exception ex)
            {
                string err = ex.Message;
                if (ex is WebException)
                    err = HttpClientHelperV45.ParseHdfsWebException(ex as WebException);
                DebugHelper.OutLog("上传文件到HDFS失败：" + err);
                DebugHelper.Error(ex, err);
                return false;
            }
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="hdfsFile">文件名（绝对路径）</param>
        /// <returns></returns>
        public static bool NewFile(string hdfsFile)
        {
            try
            {
                string dir = hdfsFile.Substring(0, hdfsFile.LastIndexOf('/'));
                if (string.IsNullOrWhiteSpace(dir)) dir = "/";
                string fileName = hdfsFile.Substring(hdfsFile.LastIndexOf('/') + 1);
                return UploadStream(new MemoryStream(), 0, 0, dir, fileName);
            }
            catch (System.Exception ex)
            {
                DebugHelper.Error(ex);
                return false;
            }
        }

        public static bool CombineFiles(string destHdfsFile, params string[] srcHdfsFiles)
        {
            try
            {
                string srcs = string.Join(",", srcHdfsFiles);
                string url = BuildUrl(destHdfsFile, WebHdfsAPI.CONCAT, "user.name=hadoop&sources=" + srcs);
                string statcode = "";
                HttpClientHelperV45.Post(url, "", out statcode);
                return "OK".Equals(statcode.ToUpper());
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return false;
            }
        }

        // Translate hostname to ip address
        private static string TranslateHostname(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;
            if (url.Contains("://"))
            {
                int start = url.IndexOf("//") + 2;
                int end = url.IndexOf(":", start);
                string hostname = url.Substring(start, end - start);
                string ip = hostname;
                if (_ipTable.ContainsKey(hostname))
                    ip = _ipTable[hostname];
                url = string.Format("http://{0}{1}", ip, url.Substring(end));
                return url;
            }
            else
            {
                if (_ipTable.ContainsKey(url))
                    return _ipTable[url];
                else
                    return url;
            }
        }



    }
}
