using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Layout;

namespace HDFSwindow
{
    public class MyLog
    {
        public static event Action<string> OnInfo;
        public static bool IsAccurateSourceClass = true;

        public static void Config()
        {
            Log4Config.LoadFileAppender();
        }

        /// <summary>
        /// 输出Info日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Info(string msg)
        {
            Type srcClass = typeof(MyLog);
            if (IsAccurateSourceClass)
            {
                var m = new StackTrace().GetFrame(1).GetMethod();
                srcClass = m.DeclaringType;
                msg = m.Name + " - " + msg;
            }
            Info(msg, srcClass);
        }

        public static void Info(string msg, object sender)
        {
            Info(msg, sender.GetType());
        }

        public static void Info(string msg, Type srcClass)
        {
            if (null == srcClass)
                srcClass = typeof(MyLog);
            log4net.ILog log = log4net.LogManager.GetLogger(srcClass);
            log.Info(msg);

            if (null != OnInfo)
                OnInfo(msg);
        }

        /// <summary>
        /// 输出错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">异常对象</param>
        public static void Error(string msg, Exception ex)
        {
            Type srcClass = typeof(MyLog);
            if (IsAccurateSourceClass)
            {
                var m = new StackTrace().GetFrame(1).GetMethod();
                srcClass = m.DeclaringType;
                msg = m.Name + " - " + msg;
            }
            Error(msg, ex, srcClass);
        }

        public static void Error(string msg, Exception ex, Object sender)
        {
            Error(msg, ex, sender);
        }

        public static void Error(string msg, Exception ex, Type srcClass)
        {
            if (null == srcClass)
                srcClass = typeof(MyLog);
            log4net.ILog log = log4net.LogManager.GetLogger(srcClass);
            log.Error(msg, ex);
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
                        "%date [%thread] %-5level %logger - %message%newline"
                    //[%property{log4net:HostName}] 
                };
                patternLayout.ActivateOptions();
                fileAppender.Layout = patternLayout;
                fileAppender.Encoding = Encoding.UTF8;
                fileAppender.LockingModel = new FileAppender.MinimalLock();
                fileAppender.ActivateOptions();

                log4net.Config.BasicConfigurator.Configure(fileAppender);

                // add console appender
                ConsoleAppender conAppender = new ConsoleAppender();
                conAppender.Name = "Console";
                conAppender.Layout = patternLayout;
                conAppender.ActivateOptions();
                log4net.Config.BasicConfigurator.Configure(conAppender);

            }
        }

    }


}
