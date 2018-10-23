using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DownloadTop100Apks
{
    class Huawei : AppInfoDownloaderBase
    {
        public Huawei(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Huawei receive start command while already running.");
                return;
            }
            mRunning = true;

            WebClient client = getWebClient();
            LinkedList<AppInfo> allAppList = new LinkedList<AppInfo>();

            for (int pageIndex = 1; pageIndex <= 5; pageIndex++)
            {
                string pageUrl = "http://appstore.huawei.com/more/all/" + pageIndex;
                string htmlCode = null;
                try
                {
                    Log.info("Fetch web page: " + pageUrl);
                    htmlCode = client.DownloadString(pageUrl);
                    string[] appInfos = htmlCode.Split(new string[] { "list-game-app dotline-btn nofloat" }, StringSplitOptions.None);
                    for (int i=1; i<appInfos.Length; i++)
                    {
                        string splitItem = appInfos[i];
                        int splitBegin = splitItem.IndexOf("href=") + "href=".Length;
                        int splitEnd = splitItem.IndexOf("<img class=\"app-ico");
                        string temp = splitItem.Substring(splitBegin, splitEnd - splitBegin).Trim();
                        string detail = "http://appstore.huawei.com" + temp.Substring(1, temp.Length-3);
                        Log.info("Fetch web page: " + detail);
                        htmlCode = client.DownloadString(detail); // detailCode

                        int sign = htmlCode.IndexOf("class=\"navsign\"");
                        int soft = htmlCode.IndexOf("value=\"3\"");
                        string SoftOrGame = sign>soft ? "Soft" : "Game";

                        int detailBegin = htmlCode.IndexOf("app-info-ul nofloat") + "app-info-ul nofloat".Length;
                        int detailEnd = htmlCode.IndexOf("app-images prsnRe nofloat");
                        string splitDetail = htmlCode.Substring(detailBegin, detailEnd-detailBegin);

                        int data1Begin = splitDetail.IndexOf("class=\"title\">") + "class=\"title\">".Length;
                        int data1End = splitDetail.IndexOf("次</span>");
                        string data1 = splitDetail.Substring(data1Begin, data1End-data1Begin).Trim();
                        string apkName = data1.Substring(0, data1.IndexOf("</span>"));
                        int installBegin = data1.IndexOf(">下载") + ">下载".Length + 1;
                        string installCountStr = data1.Substring(installBegin, data1.Length - installBegin);
                        long installCount = long.Parse(installCountStr);

                        int data2Begin = splitDetail.IndexOf("app-info-ul nofloat") + "app-info-ul nofloat".Length;
                        int data2End = splitDetail.IndexOf("bdsharebuttonbox");
                        string data2 = splitDetail.Substring(data2Begin, data2End - data2Begin).Trim();
                        int apkSizeBegin = data2.IndexOf("<span>") + "<span>".Length;
                        int apkSizeEnd = data2.IndexOf("</span>");
                        string apkSize = data2.Substring(apkSizeBegin, apkSizeEnd-apkSizeBegin);
                        int companyBegin = data2.IndexOf("title=") + "title=".Length + 1;
                        int companyEnd = data2.IndexOf("'>");
                        string company = data2.Substring(companyBegin, companyEnd - companyBegin);

                        int data3Begin = splitDetail.IndexOf("zhytools.downloadApp") + "zhytools.downloadApp".Length;
                        int data3End = splitDetail.IndexOf("class=\"b-lt");
                        string data3 = splitDetail.Substring(data3Begin, data3End - data3Begin).Trim();
                        data3 = data3.Substring(1, data3.IndexOf(")") - 1).Trim();
                        string[] tempArr = data3.Split(new string[] { "," }, StringSplitOptions.None);
                        string categoryTemp = tempArr[4];
                        string category = categoryTemp.Substring(1, categoryTemp.Length-2);
                        string downloadUrlTemp = tempArr[5].Trim();
                        string downloadUrl = downloadUrlTemp.Substring(1, downloadUrlTemp.Length - 2).Trim();
                        downloadUrl = downloadUrl.Replace("&#x2F;", "/");

                        int pkgBegin = downloadUrl.LastIndexOf("/") + "/".Length;
                        int pkgEnd = downloadUrl.IndexOf(".apk?");
                        string pkgTemp = downloadUrl.Substring(pkgBegin, pkgEnd - pkgBegin);
                        string pkgName = pkgTemp.Substring(0, pkgTemp.LastIndexOf("."));
                        string versionTemp = tempArr[6];
                        string version = versionTemp.Substring(1, versionTemp.Length - 2);

                        AppInfo appInfo = new AppInfo(apkName, pkgName, version, apkSize, category, company, -1, SoftOrGame, downloadUrl, "Huawei");
                        allAppList.AddLast(appInfo);

                    } // end for appInfos

                } // end try
                catch (Exception ex)
                {
                    Log.info("Huawei downloader broken, need fix by code change!");
                    Log.info(ex.Message);
                    Log.info(ex.StackTrace);
                } // end catch
            } // end for pageIndex

            // getTop100
            for (int i = 0; i < 100; i++)
            {
                HuaweiList.AddLast(allAppList.ElementAt(i));
            }

            done("Huawei");

        }

        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Huawei downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }
    }
}
