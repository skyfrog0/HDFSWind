using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;

namespace HDFSwindow
{
    /// <summary>
    /// 调试日志工具
    /// </summary>
    public class DebugHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void OutputDebugString(string message);
        
        /// <summary>
        /// 内存缓冲区
        /// </summary>
        public static MemoryStream LogStream = new MemoryStream();
        public static Object _streamMtx = new Object();

        /// <summary>
        /// 是否允许控制台输出
        /// </summary>
        public static bool IsConsole { get; set; }

        /// <summary>
        /// 写入内存缓存日志
        /// </summary>
        /// <param name="msg">日志文本</param>
        public static void WriteBuffer(string msg)
        {
            if (!LogStream.CanWrite)
                return;

            msg = string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("HH:mm:ss.fff"), msg);
            byte[] bts = Encoding.UTF8.GetBytes(msg);
            lock (_streamMtx)
            {
                LogStream.Write(bts, 0, bts.Length);
            }
        }

        /// <summary>
        /// 读取内存缓存日志（最大1024 Byte），清空缓存区
        /// </summary>
        /// <returns>缓存区中的日志内存</returns>
        public static string ReadBufferOnce()
        {
            if (!LogStream.CanRead) return null;
            if (LogStream.Length <= 0)
                return null;

            //return string.Format("Capacity={0},Length={1},Position={2}, R={3},W={4},S={5}\n",
            //    LogStream.Capacity, LogStream.Length, LogStream.Position, LogStream.CanRead, LogStream.CanWrite, LogStream.CanSeek);

            lock (_streamMtx)
            {
                LogStream.Seek(0, SeekOrigin.Begin);
                byte[] bts = new byte[4 * 1024];
                int len = LogStream.Read(bts, 0, bts.Length);
                if (len > 0)
                {
                    LogStream.SetLength(0);
                    return Encoding.UTF8.GetString(bts);
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 输出调试信息，且同时记录日志文件
        /// </summary>
        /// <param name="message"></param>
        public static void OutLog(string message)
        {
            Output(message);
            Log(message);
        }

        public static void Error(Exception ex)
        {
            string message = "异常：" + ex.Message;
            MyLog.Error(message, ex);
        }

        public static void Error(Exception ex, String message)
        {
            MyLog.Error(message, ex);
        }

#if DEBUG

        /// <summary>
        /// 输出调试信息
        /// </summary>
        /// <param name="message">字符串信息</param>
        public static void Output(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            string strPrefix = "-- ";
            OutputDebugString(strPrefix + message);

            if (IsConsole) Console.WriteLine(message);

            WriteBuffer(message);
        }

        // 日志工具实例


        // 记录调试日志
        public static void Log(string message)
        {
            MyLog.Info(message);
        }

#else
        public static void Output(string message) 
        {
            if(IsConsole) Console.WriteLine(message);
            WriteBuffer(message);
        }
        public static void Log(string message)
        {
            MyLog.Info(message);
        }
#endif

    }

}
