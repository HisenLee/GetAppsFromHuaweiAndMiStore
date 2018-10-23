using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadTop100Apks
{
    class Config
    {
        public static string OUT_PUT_DIR = "C:\\" + typeof(Config).Namespace + "\\";
        public static string PROXY_SERVER = null;
        public static int PROXY_PORT = 0;

        
        public static bool loadConfiguration()
        {
            // read
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "DownloadTop100Apks.cfg");
            }
            catch (Exception ex)
            {
                Log.info("Read config file failed!");
                Log.info(ex.Message);
                return false;
            }

            if (lines != null)
            {
                foreach (string s in lines)
                {
                    if (s != null)
                    {
                        if (s.Length <= 0 || s.StartsWith("#"))
                        {
                            continue;
                        }
                        else
                        {
                            string[] keyValuePair = s.Trim().Split('=');
                            if (keyValuePair.Length <= 1)
                            {
                                Log.info("Config item invalid: " + s);
                                continue;
                            }
                            else if (keyValuePair.Length == 2)
                            {
                                try
                                {
                                    string key = keyValuePair[0];
                                    string value = keyValuePair[1];
                                    if (value != null && value.Length > 0)
                                    {
                                        applyConfigFromKeyValuePair(key, value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.info("Config invalid, Line: " + s);
                                    Log.info(ex.Message);
                                    return false;
                                }
                            }

                        }
                    }
                }
            }

            return true;

        }


        private static void applyConfigFromKeyValuePair(string k, string v)
        {
            Log.info("Configration " + k + "=" + v);
            switch (k)
            {
                case "OUT_PUT_DIR":
                    if (!Directory.Exists(@v))
                    {
                        Directory.CreateDirectory(@v);
                    }
                    OUT_PUT_DIR = v;
                    if (!OUT_PUT_DIR.EndsWith("\\"))
                    {
                        OUT_PUT_DIR = OUT_PUT_DIR + "\\";
                    }
                    break;
                case "NETWORK_PROXY":
                    try
                    {
                        int index = v.LastIndexOf(':');
                        PROXY_SERVER = v.Substring(0, index);
                        string portString = v.Substring(index + 1, v.Length - index);
                        PROXY_PORT = int.Parse(portString);
                    }
                    catch (Exception)
                    {
                        Log.info("NETWORK_PROXY setting invalid, fallback to system default.");
                        Log.info(k + "=" + v);
                    }
                    break;
            }
        }

    }
}
