using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HDFSwindow.HDFSCore
{
    /// <summary>
    /// Web请求任务的响应结果
    /// </summary>
    public class GAWebJobResult
    {
        public string JobID;
        public byte[] Result;
        public bool IsSucceed;
        public string ErrorLog;
    }

    /// <summary>
    /// HTTP响应结果
    /// </summary>
    public class HttpResult
    {
        public int Code { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string ErrorLog { get; set; }        

        public string GetStatus()
        {
            return ((HttpStatusCode)Code).ToString();
        }

        public string GetContentText()
        {
            if (null != Content)
                return Encoding.UTF8.GetString(Content);
            else
                return "";
        }

        public void SetContent(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                Content = Encoding.UTF8.GetBytes(text);
        }

    }

    public class HttpClientHelperV45
    {
        public static int CONN_TIMEOUT = 3000;     //建立网络连接并等到首个响应包的超时，默认3s
        public const int IO_TIMEOUT = 3600000;      //读写网络流的超时时长，默认 1 hour

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Get(string url, out string statusCode)
        {
            try
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(
                  new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
                return null;
            }
            catch (WebException webex)
            {
                var resp = (HttpWebResponse)webex.Response;
                string err = webex.Message;
                if (null != resp)
                    err = string.Format("{0} {1}", (int)resp.StatusCode, resp.StatusCode);
                DebugHelper.OutLog(err);
                statusCode = resp.StatusCode.ToString();
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                statusCode = "-1";
                return null;
            }
        }

        public static string Get(string url)
        {
            string temp = "";
            return Get(url, out temp);
        }

        public static byte[] GetBytes(string url, out string statusCode)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode)
            {
                Task<byte[]> t = response.Content.ReadAsByteArrayAsync();
                return t.Result;
            }
            else
                return null;
        }

        public static void GetAsync(string url, string id, Action<GAWebJobResult> callback)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            httpClient.GetAsync(url).ContinueWith(reqTask =>    //异步请求
            {
                HttpResponseMessage response = reqTask.Result;
                bool bOk = response.IsSuccessStatusCode;
                byte[] result = response.Content.ReadAsByteArrayAsync().Result;         //阻塞IO
                // 执行回调
                callback(new GAWebJobResult()
                {
                    JobID = id,
                    IsSucceed = bOk,
                    Result = result,
                    ErrorLog = response.ReasonPhrase
                });
            });
        }

        public static HttpResult Request(string url, TimeSpan timeout, 
            HttpMethod method = HttpMethod.GET,
            string contentType = "application/json")
        {
            try
            {                
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                var httpClient = new HttpClient();
                httpClient.Timeout = timeout;

                HttpRequestMessage reqmsg = new HttpRequestMessage(ConvertMethodType(method), url);

                HttpResponseMessage response = httpClient.SendAsync(reqmsg).Result;
                HttpResult result = new HttpResult();
                result.Code = (int)response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    Task<byte[]> t = response.Content.ReadAsByteArrayAsync();
                    result.Content = t.Result;
                    result.ContentType = response.Content.Headers.ContentType.ToString();
                }
                return result;
            }
            catch (WebException webex)
            {
                var resp = (HttpWebResponse)webex.Response;
                string err = webex.Message;
                if (null != resp)
                    err = string.Format("{0} {1}", (int)resp.StatusCode, resp.StatusCode);
                DebugHelper.OutLog(err);
                return new HttpResult
                {
                    Code = (int)resp.StatusCode,
                    ErrorLog = webex.Message
                };
            }
            catch (Exception ex)
            {
                return new HttpResult
                {
                    Code = -1,
                    ErrorLog = ex.Message
                };
            }
        }

        private static System.Net.Http.HttpMethod ConvertMethodType(HttpMethod method)
        {
            switch (method)
            {
                case HttpMethod.GET:
                    return System.Net.Http.HttpMethod.Get;
                case HttpMethod.POST:
                    return System.Net.Http.HttpMethod.Post;
                case HttpMethod.PUT:
                    return System.Net.Http.HttpMethod.Put;
                case HttpMethod.DELETE:
                    return System.Net.Http.HttpMethod.Delete;
                default:
                    return System.Net.Http.HttpMethod.Get;
            }
        }

        /// <summary>
        /// 执行Post请求，返回结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string Post(string url, string postData, out string statusCode)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }

            return null;
        }

        /// <summary>
        /// 发起Post请求，返回JSON解析得到的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static T PostBackJsonObject<T>(string url, string postData)
            where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }
            return result;
        }


        public static string Put(string url, string content = "")
        {
            try
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(
                  new MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent httpContent = new StringContent(content);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpContent.Headers.ContentType.CharSet = "utf-8";

                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;
                string statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
                return null;
            }
            catch (WebException webex)
            {
                var resp = (HttpWebResponse)webex.Response;
                string err = webex.Message;
                if (null != resp)
                    err = string.Format("{0} {1}", (int)resp.StatusCode, resp.StatusCode);
                DebugHelper.OutLog(err);
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }
        
        public static string Delete(string url)
        {
            try
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(
                  new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
                string statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
                return null;
            }
            catch (WebException webex)
            {
                var resp = (HttpWebResponse)webex.Response;
                string err = webex.Message;
                if (null != resp)
                    err = string.Format("{0} {1}", (int)resp.StatusCode, resp.StatusCode);
                DebugHelper.OutLog(err);
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);
                return null;
            }
        }


        #region WebRequest Wrapper
        
        /// <summary>
        /// 执行Http请求，上传二进制流数据（最大超时1h）
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="ins">输入流数据</param>
        /// <param name="method">HTTP请求方式</param>
        /// <returns></returns>
        public static string HttpUpload(string url, Stream ins, HttpMethod method = HttpMethod.PUT)
        {
            string result = null;
            HttpWebReqWrapper.Call(url, method,
                // 请求参数设置
                (HttpWebRequest req) =>
                {
                    //req.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";  // 特定标头不能这么设置
                    req.ContentType = "application/octet-stream";
                    req.Timeout = IO_TIMEOUT;
                    req.ReadWriteTimeout = IO_TIMEOUT;
                    req.SendChunked = true;
                    using (Stream outstream = req.GetRequestStream())
                    {
                        int c = 1024 * 8;
                        byte[] buffer = new byte[c];
                        int l = ins.Read(buffer, 0, c);
                        while (l > 0)
                        {
                            outstream.Write(buffer, 0, l);
                            outstream.Flush();
                            l = ins.Read(buffer, 0, c);
                        }
                        outstream.Close();
                    }
                    return req;
                },
                // 响应结果提取
                (HttpWebResponse rep) =>
                {
                    using (Stream outstream = rep.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(outstream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                        reader.Close();
                    }
                    return rep;
                });
            return result;
        }

        public static string HttpUpload(string url, Stream ins, long offset, long length, 
            HttpMethod method = HttpMethod.PUT)
        {
            try
            {
                if (url.StartsWith("https"))
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                string result = null;
                HttpWebReqWrapper.Call(url, method,
                    // 请求参数设置
                    (HttpWebRequest req) =>
                    {
                        //req.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";  // 特定标头不能这么设置
                        req.ContentType = "application/octet-stream";
                        req.Timeout = IO_TIMEOUT;
                        req.ReadWriteTimeout = IO_TIMEOUT;
                        req.SendChunked = true;
                        using (Stream outstream = req.GetRequestStream())
                        {
                            ins.Seek(offset, SeekOrigin.Begin);
                            long endPos = offset + length;
                            int c = 1024 * 8;
                            byte[] buffer = new byte[c];
                            while (ins.Position < endPos)
                            {
                                int l = ins.Read(buffer, 0, c);
                                if (ins.Position > endPos)
                                    l = l - (int)(ins.Position - endPos);
                                if (l <= 0)
                                    break;
                                outstream.Write(buffer, 0, l);
                                outstream.Flush();
                            }                            
                            outstream.Close();
                        }
                        return req;
                    },
                    // 响应结果提取
                    (HttpWebResponse rep) =>
                    {
                        using (Stream outstream = rep.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(outstream, Encoding.UTF8);
                            result = reader.ReadToEnd();
                            reader.Close();
                        }
                        return rep;
                    });
                return result;
            }
            catch (WebException webex)
            {
                var resp = (HttpWebResponse)webex.Response;
                string err = webex.Message;
                if (null != resp)
                    err = string.Format("{0} {1}", (int)resp.StatusCode, resp.StatusCode);
                DebugHelper.OutLog(err);
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.Error(ex);  //DebugHelper.OutLog(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取服务重定向后的URL地址，便于手动处理重定向
        /// </summary>
        /// <param name="url">请求的服务地址</param>
        /// <param name="method">请求方法</param>
        /// <returns></returns>
        public static string GetRedirectedURL(string url, HttpMethod method)
        {
            try
            {
                string redirectUrl = null;
                HttpWebReqWrapper.Call(url, method,
                    // 请求参数设置
                    (HttpWebRequest req) =>
                    {
                        //req.Headers[HttpRequestHeader.Authorization] = "token";
                        req.AllowAutoRedirect = false;
                        return req;
                    },
                    // 响应结果提取
                    (HttpWebResponse rep) =>
                    {
                        int code = (int)rep.StatusCode;
                        if (307 == code || 301 == code)
                        {
                            redirectUrl = rep.Headers[HttpResponseHeader.Location];
                        }
                        return rep;
                    });

                return redirectUrl;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                if (ex is WebException)
                {
                    err = ParseHdfsWebException(ex as WebException);
                }
                DebugHelper.Error(ex);
                return null;
            }
        }
        
        #endregion


        /// <summary>
        /// 解析Web请求异常
        /// </summary>
        /// <param name="exp">异常对象</param>
        /// <returns></returns>
        public static string ParseHdfsWebException(WebException exp)
        {
            string err = "";
            try
            {
                var resp = exp.Response as HttpWebResponse;
                err = string.Format("{0} {1}.", (int)resp.StatusCode, resp.StatusCode);

                using (Stream outstream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(outstream, Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    reader.Close();
                    if (!string.IsNullOrWhiteSpace(result) && result.Contains("RemoteException"))
                    {
                        // WebHDFS error json
                        var jobj = JsonConvert.DeserializeObject(result) as JObject;
                        string msg = jobj["RemoteException"]["message"].ToString();
                        err += " error: " + msg;
                    }
                    else
                        err += result;
                }

            }
            catch (Exception) { }
            if (string.IsNullOrWhiteSpace(err))
                err = exp.Message;
            return err;
        }


    }
}
