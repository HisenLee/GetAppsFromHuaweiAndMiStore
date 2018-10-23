using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace DownloadTop100Apks
{
    abstract class AppInfoDownloaderBase
    {
        private string mOutDir = null;
        public bool mRunning = false;

        public LinkedList<AppInfo> MiStoreList = new LinkedList<AppInfo>();
        public LinkedList<AppInfo> HuaweiList = new LinkedList<AppInfo>();

        private WebClient mWebClient = null;
        public WebClient getWebClient()
        {
            if (mWebClient == null)
            {
                mWebClient = new WebClient();
                mWebClient.Encoding = Encoding.UTF8;
                if (Config.PROXY_PORT > 0)
                {
                    WebProxy proxy = new WebProxy(Config.PROXY_SERVER, Config.PROXY_PORT);
                    mWebClient.Proxy = proxy;
                }
            }
            return mWebClient;
        }

        public abstract void start();

        public abstract void stop();

        public void setOutDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                if (!Directory.Exists(dir))
                {
                    Log.info("Out dir not exist!");
                    Log.info("Out dir: " + dir);
                }
            }
            mOutDir = dir;
            if (!mOutDir.EndsWith("\\"))
            {
                mOutDir = mOutDir + "\\";
            }
        }

        public void done(string store)
        {
            // step1: generate Excel
            string outDir = mOutDir;
            LinkedList<AppInfo> appList = new LinkedList<AppInfo>();
           
            if (store.Equals("MiStore"))
            {
                appList = MiStoreList;
            }
            else
            {
                appList = HuaweiList;
            }
          
            List<string[]> rows = new List<string[]>();
            int rank = 1;
            foreach (AppInfo appItem in appList)
            {
                appItem.app_ranking = rank++;
                rows.Add(appItem.toAllAppsExcelRow());           
            }

            ExcelWriter exs = new ExcelWriter(outDir + store, new string[] { store });
            exs.writeHeadRow(1, AppInfo.fullExcelHeader);
            exs.writeRows(1, rows, 2);
            exs.saveAndClose();

            // step2: download Top100 apks
            new DownloadAction().downloadBatchApps(outDir, appList);
            // step3: change flag
            while (!DownloadAction.isFinishedAll)
            {
                Thread.Sleep(1000);
            }
            mRunning = false;

        }





    }
}
