using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DownloadTop100Apks
{
    class MiStore : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
        {
           "实用工具", "影音视听", "聊天社交", "图书阅读", "学习教育", "效率办公",
           "时尚购物", "居家生活", "旅行交通", "摄影摄像", "医疗健康", "体育运动",
           "新闻资讯", "娱乐消遣", "金融理财"
        };

        private string baseUrl = "http://app.mi.com";

        public MiStore(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("MiStore receive start command while already running.");
                return;
            }
            mRunning = true;

            WebClient client = getWebClient();
            LinkedList<AppInfo> allAppList =  new LinkedList<AppInfo>(); 

            for (int pageIndex=1; pageIndex <= 3; pageIndex++)
            {
                string pageUrl = baseUrl + "/topList?page=" + pageIndex;
                string htmlCode = null;
                try
                {
                    Log.info("Fetch web page: " + pageUrl);
                    htmlCode = client.DownloadString(pageUrl);

                    int tempBegin = htmlCode.IndexOf("class=\"applist\">");
                    int tempEnd = htmlCode.IndexOf("class=\"pages\"");

                    string tempNowPageCode = htmlCode.Substring(tempBegin, tempEnd - tempBegin);

                    string[] appInfos = tempNowPageCode.Split(new string[] { "<li>" }, StringSplitOptions.None);
                    for (int i=1; i<appInfos.Length; i++)
                    {
                        string item = appInfos[i];
                        int tempBegin2 = item.IndexOf("href=\"")+ "href=\"".Length;
                        int tempEnd2 = item.IndexOf("<img data-src=");
                        string detailTemp = item.Substring(tempBegin2, tempEnd2 - tempBegin2).Trim();
                        string detail = baseUrl + detailTemp.Substring(0, detailTemp.Length - 2);

                        Log.info("Fetch web page: " + detail);
                        htmlCode = client.DownloadString(detail); // detailCode

                        int detailBegin1 = htmlCode.IndexOf("intro-titles\">") + "intro-titles\">".Length;
                        int detailEnd1 = htmlCode.IndexOf("class=\"download\">直接下载");
                        string detailStr1 = htmlCode.Substring(detailBegin1, detailEnd1- detailBegin1).Trim();
                        int companyBegin = detailStr1.IndexOf("<p>") + "<p>".Length;
                        int companyEnd = detailStr1.IndexOf("</p>");
                        string company = detailStr1.Substring(companyBegin, companyEnd - companyBegin).Trim();
                        int apkNameBegin = detailStr1.IndexOf("<h3>") + "<h3>".Length;
                        int apkNameEnd = detailStr1.IndexOf("</h3>");
                        string apkName = detailStr1.Substring(apkNameBegin, apkNameEnd - apkNameBegin).Trim();
                        int categoryBegin = detailStr1.IndexOf("</b>") + "</b>".Length;
                        int categoryEnd = detailStr1.IndexOf("<span");
                        string category = detailStr1.Substring(categoryBegin, categoryEnd - categoryBegin).Trim();
                        int downloadUrlBegin = detailStr1.IndexOf("href=\"") + "href=\"".Length;
                        int downloadUrlEnd = detailStr1.LastIndexOf("\"");
                        string downloadUrl = baseUrl + detailStr1.Substring(downloadUrlBegin, downloadUrlEnd - downloadUrlBegin).Trim();
                        string SoftOrGame = url_app_categories.Contains(category) ? "Soft" : "Game";

                        int detailBegin2 = htmlCode.IndexOf("details preventDefault") + "details preventDefault".Length;
                        int detailEnd2 = htmlCode.LastIndexOf("special-li");
                        string detailStr2 = htmlCode.Substring(detailBegin2, detailEnd2 - detailBegin2).Trim();
                        int apkSizeBegin = detailStr2.IndexOf("软件大小:</li>") + "软件大小:</li>".Length;
                        int apkSizeEnd = detailStr2.IndexOf(">版本号");
                        string apkSizeTemp = detailStr2.Substring(apkSizeBegin, apkSizeEnd-apkSizeBegin).Trim();
                        string apkSize = apkSizeTemp.Substring(apkSizeTemp.IndexOf("<li>") + "<li>".Length, apkSizeTemp.IndexOf("</li>") - apkSizeTemp.IndexOf("<li>") - "<li>".Length);
                        int versionBegin = detailStr2.IndexOf(">版本号") + ">版本号".Length;
                        int versionEnd = detailStr2.IndexOf(">更新时间");
                        string versionTemp = detailStr2.Substring(versionBegin, versionEnd - versionBegin).Trim();
                        string version = versionTemp.Substring(versionTemp.IndexOf("<li>") + "<li>".Length, versionTemp.LastIndexOf("</li>") - versionTemp.IndexOf("<li>") - "<li>".Length);
                        int pkgNameBegin = detailStr2.IndexOf("special-li\">")+ "special-li\">".Length;
                        int pkgNameEnd = detailStr2.IndexOf(">appId");
                        string pkgNameTemp = detailStr2.Substring(pkgNameBegin, pkgNameEnd-pkgNameBegin).Trim();
                        string pkgName = pkgNameTemp.Substring(0, pkgNameTemp.IndexOf("</li>"));

                        AppInfo appInfo = new AppInfo(apkName, pkgName, version, apkSize, category, company, -1, SoftOrGame, downloadUrl, "MiStore");
                        allAppList.AddLast(appInfo);
                    }


                }
                catch (Exception ex)
                {
                    Log.info("MiStore downloader broken, need fix by code change!");
                    Log.info(ex.Message);
                    Log.info(ex.StackTrace);
                }

            } // end  for pageIndex

            // getTop100
            for (int i=0; i<100; i++)
            {
                MiStoreList.AddLast(allAppList.ElementAt(i));
            }

            done("MiStore");


        } // end  method start

        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("MiStore downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }


    }
}
