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

        private static MyLogImpl _logger = new MyLogImpl(typeof(DebugHelper));

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
            _logger.Error(message, ex);
        }

        public static void Error(Exception ex, String message)
        {
            _logger.Error(message, ex);
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
            _logger.Info(message);
        }

#else
        public static void Output(string message) 
        {
            if(IsConsole) Console.WriteLine(message);
            WriteBuffer(message);
        }
        public static void Log(string message)
        {
            _logger.Info(message);
        }
#endif

    }

    /// <summary>
    /// ref:  https://blog.csdn.net/binnygoal/article/details/79557746
    /// </summary>
    public class MyLogImpl
    {
        private static Type _thisDeclaringType = typeof(MyLogImpl);
        private static log4net.ILog _log = log4net.LogManager.GetLogger(_thisDeclaringType);

        public MyLogImpl(Type srcClass)
        {
            _thisDeclaringType = srcClass;
            _log = log4net.LogManager.GetLogger(srcClass);
        }

        public void Info(string msg)
        {
            _log.Info(msg);
        }

        public void Error(string msg, Exception ex)
        {
            _log.Error(msg, ex);
        }

        public void Error(int operatorID, string operand, int actionType, object message, string ip, string browser, string machineName, System.Exception t)
        {
            if (_log.IsErrorEnabled)
            {
                LoggingEvent loggingEvent = new LoggingEvent(_thisDeclaringType, _log.Logger.Repository, _log.Logger.Name, Level.Info, message, t);
                loggingEvent.Properties["Operator"] = operatorID;
                loggingEvent.Properties["Operand"] = operand;
                loggingEvent.Properties["ActionType"] = actionType;
                loggingEvent.Properties["IP"] = ip;
                loggingEvent.Properties["Browser"] = browser;
                loggingEvent.Properties["MachineName"] = machineName;
                _log.Logger.Log(loggingEvent);
            }
        }
    }

    public class Log4Config
    {

        /// <summary>
        /// 使用文本文件记录异常日志
        /// </summary>
        public static void LoadFileAppender()
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            currentPath = Path.Combine(currentPath, @"Log");
            string txtLogPath = Path.Combine(currentPath, "Log-" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

             log4net.Repository.Hierarchy.Hierarchy hier =
                (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            if (hier != null)
            {
                FileAppender fileAppender = new FileAppender();
                fileAppender.Name = "LogFileAppender";
                fileAppender.File = txtLogPath;
                fileAppender.AppendToFile = true;
                PatternLayout patternLayout = new PatternLayout
                {
                    ConversionPattern =
                        "%date  [%thread] %-5level %logger - %message%newline" 
                        //at:%property{HostName}
                };
                patternLayout.ActivateOptions();
                fileAppender.Layout = patternLayout;
                fileAppender.Encoding = Encoding.UTF8;
                fileAppender.ActivateOptions();

                log4net.Config.BasicConfigurator.Configure(fileAppender);
            }
        }
    
    }

}
