using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDFSwindow
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            //log4net.Config.XmlConfigurator.Configure();
            Log4Config.LoadFileAppender();

            Application.Run(new FormHDFS());
        }
    }
}
