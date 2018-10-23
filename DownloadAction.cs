using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace DownloadTop100Apks
{
    class DownloadAction
    {
        public static string mOutFile = null;

        public static int mTotalFile = 0;
        private int mFinishedFile = 0;

        private AppInfo mApp = null;

        public static bool isFinishedAll;

        public void downloadBatchApps(string outDir, LinkedList<AppInfo> appList)
        {
            isFinishedAll = false;

            mOutFile = outDir;
            mTotalFile = appList.Count;

            
            Thread thread = new Thread(new ThreadStart(() => createWebClient(appList)));
            thread.Start();
        }

        private void createWebClient(LinkedList<AppInfo> appList)
        {

            foreach (AppInfo appItem in appList)
            {
                mApp = appItem;

                WebClient webClient = new WebClient();
                if (Config.PROXY_PORT > 0)
                {
                    WebProxy proxy = new WebProxy(Config.PROXY_SERVER, Config.PROXY_PORT);
                    webClient.Proxy = proxy;
                }

                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadFileCompleted);

                int rank = mApp.app_ranking;
                string ranking = "";
                if (rank < 10)
                {
                    ranking = "00" + rank;
                }
                else if (rank < 100)
                {
                    ranking = "0" + rank;
                }
                else
                {
                    ranking = rank+"";
                }
                string fileName = mOutFile + ranking + "_" + mApp.apkName + "_" + mApp.pkgName + "_.apk";

                webClient.DownloadFileAsync(new Uri(mApp.downloadUrl), fileName);
            }
        }    

        private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           // Log.info(string.Format("Download " + mApp.apkName + " process {0}%  {1}/{2}(bytes)", e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive));
        }

        private void downloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            mFinishedFile++;

            int percent = (int)(100.0 * mFinishedFile / mTotalFile);
            Log.info(string.Format("{0}/{1} Apks download finished", mFinishedFile, mTotalFile));

            if (mFinishedFile == mTotalFile)
            {
                isFinishedAll = true;
            }

            if (sender is WebClient)
            {
                ((WebClient)sender).CancelAsync();
                ((WebClient)sender).Dispose();
            }
        }


    }
}
