using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTop100Apks
{
    class AppInfo
    {
        public string apkName;
        public string pkgName;
        public string version;
        public string apkSize;
        public string category;
        public string company;
        public long installCount;
        public string SoftOrGame;
        public string downloadUrl;
        public string store;

        public int app_ranking;

        public AppInfo(string apkName, string pkgName, string version, string apkSize, string category, string company, long installCount, string SoftOrGame, string downloadUrl, string store)
        {
            this.apkName = apkName;
            this.pkgName = pkgName;
            this.version = version;
            this.apkSize = apkSize;
            this.category = category;
            this.company = company;
            this.installCount = installCount;
            this.SoftOrGame = SoftOrGame;
            this.downloadUrl = downloadUrl;
            this.store = store;
        }

        public AppInfo(string apkName, string downloadUrl)
        {
            this.apkName = apkName;
            this.downloadUrl = downloadUrl;
        }

        public static string[] fullExcelHeader = {"app_ranking", "app_name", "app_version", "package_name", "category", "apkSize", "company", "installCount", "downloadUrl", "isSoft", "store"};

        public string[] toAllAppsExcelRow()
        {
            string[] rowData = new string[]{"" + app_ranking, apkName, version, pkgName, category, apkSize, company, "N/A", downloadUrl, SoftOrGame, store };
            return rowData;
        }
    }
}
