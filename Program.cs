using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadTop100Apks
{
    class Program
    {
        private static string mOutDir = Config.OUT_PUT_DIR;

        private static AppInfoDownloaderBase Huawei = null;
        private static AppInfoDownloaderBase MiStore = null;

        static void Main(string[] args)
        {
            // read from DownloadTop100Apks.cfg
            if (!Config.loadConfiguration())
            {
                Log.info("Error loading configuraion!");
                return;
            }
            // set out dir
            mOutDir = Config.OUT_PUT_DIR;

            Huawei = new Huawei(mOutDir + "Huawei");
            new Thread(new ThreadStart(Huawei.start)).Start();
            Thread.Sleep(1000);
            // wait for app info downloader finish.
            while (Huawei.mRunning)
            {
                Thread.Sleep(1000);
            }

            MiStore = new MiStore(mOutDir + "MiStore");
            new Thread(new ThreadStart(MiStore.start)).Start();
            Thread.Sleep(1000);
            // wait for app info downloader finish.
            while (MiStore.mRunning)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("All apks finished under " + mOutDir + ", Press any key to quit...");
            Console.ReadKey();
        }
    }
}
