using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HDFSwindow.HDFSCore
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// 回调函数：设置Http选项
    /// </summary>
    /// <param name="req">http请求对象</param>
    /// <returns>http请求对象</returns>
    public delegate HttpWebRequest HttpReqConfigCallback(HttpWebRequest req);

    public delegate HttpWebResponse HttpResponseHandler(HttpWebResponse response);

    /// <summary>
    /// HttpWebRequest 包装类
    /// </summary>
    public static class HttpWebReqWrapper
    {
        public static int CONN_TIMEOUT = 3000; //建立网络连接并等到首个响应包的超时，默认3s

        static HttpWebReqWrapper()
        {
            ServicePointManager.DefaultConnectionLimit = 256;   //最大并发连接数
            ServicePointManager.Expect100Continue = false;      //发送大数据时，有些HTTP服务端不支持Expect: 100-continue

        }

        public static string Get(string url)
        {
            return CommonHttpRequest(url, HttpMethod.GET);
        }

        public static string Post(string url, string data)
        {
            return CommonHttpRequest(url, HttpMethod.POST, data);
        }

        public static string Put(string url, string data)
        {
            return CommonHttpRequest(url, HttpMethod.PUT, data);
        }

        public static void Delete(string url)
        {
            CommonHttpRequest(url, HttpMethod.DELETE);
        }

        /// <summary>
        /// 自定义HTTP请求处理
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="type">请求方式</param>
        /// <param name="reqConfiger">请求设置函数</param>
        /// <param name="resultParser">响应提取函数</param>
        public static void Call(string url, HttpMethod type, HttpReqConfigCallback reqConfiger, HttpResponseHandler resultParser)
        {
            if (string.IsNullOrWhiteSpace(url) || null == reqConfiger || null == resultParser)
                throw new ArgumentNullException();
            try
            {
                //构造http请求的对象
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                //设置
                req.Method = type.ToString();
                req.Timeout = CONN_TIMEOUT;
                reqConfiger(req);
                // 发送请求，获得响应结果
                var t1 = DateTime.Now;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    resultParser(response);
                    response.Close();
                    // LOG
                   /* HttpStatusCode eStatus = response.StatusCode;
                    var millis = (DateTime.Now - t1).TotalMilliseconds;
                    string info = string.Format("HTTP-Resp {0} {1} by {2}@{3}. time: {4} ms", (int)eStatus, eStatus, response.Server, req.Host, millis);
                    DebugHelper.OutLog(info);*/
                }
                req.Abort();
            }
            catch (Exception ex)
            {
                throw ex;   //new Exception("HTTP请求异常", ex);
            }
        }

        private static string CommonHttpRequest(string url, HttpMethod type, string data = "", HttpReqConfigCallback cfgFunc = null)
        {
            HttpWebRequest myRequest = null;
            HttpWebResponse myResponse = null;
            try
            {
                //构造http请求的对象
                myRequest = (HttpWebRequest)WebRequest.Create(url);
                //DebugHelper.OutLog(">>" + url);
                //设置
                if (null == cfgFunc)
                    cfgFunc = new HttpReqConfigCallback(DeafultHttpConfigFunc);
                cfgFunc(myRequest);
                myRequest.Method = type.ToString();

                if (!string.IsNullOrEmpty(data))
                {
                    if (HttpMethod.POST == type)
                    {
                        // 数据写入请求Body
                        byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
                        myRequest.ContentLength = buf.Length;
                        using (Stream outstream = myRequest.GetRequestStream())
                        {
                            outstream.Write(buf, 0, buf.Length);
                            outstream.Flush();
                            outstream.Close();
                        }
                    }
                }

                // 发送请求，获得响应结果
                var t1 = DateTime.Now;
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                string result = null;
                using (Stream outstream = myResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(outstream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                    reader.Close();
                }

                // LOG
                /*HttpStatusCode eStatus = myResponse.StatusCode;
                var millis = (DateTime.Now - t1).TotalMilliseconds;
                string info = string.Format("HTTP-Resp {0} {1} by {2}@{3}. time: {4} ms", (int)eStatus, eStatus, myResponse.Server, myRequest.Host, millis);
                DebugHelper.OutLog(info);*/

                // Release
                myResponse.Close();
                myRequest.Abort();
                return result;
            }
            catch (Exception e)
            {
                if (myResponse != null) myResponse.Close();
                if (myRequest != null) myRequest.Abort();
                throw e;
            }
        }

        private static HttpWebRequest DeafultHttpConfigFunc(HttpWebRequest req)
        {
            req.ProtocolVersion = HttpVersion.Version11;
            req.Timeout = CONN_TIMEOUT;
            req.AllowAutoRedirect = true;
            req.KeepAlive = false;
            req.ContentType = "application/json";   //"text/plain";
            return req;
        }

    }


}
