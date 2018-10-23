using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTop100Apks
{
    class Log
    {
        public static void info(string text)
        {
            string content = DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "  " + text;
            Console.WriteLine(content);
        }
    }
}
