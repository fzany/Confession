using Newtonsoft.Json;
using Store.Helpers;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Uwp.Helpers
{
    public class Logic
    {
        public static string[] Categories = { "Love", "Sex", "Family",
            "Food", "Religion","Travel",
            "General","Money", "Health",
            "Crime" ,"Hilarious"};

        internal static string GetToken()
        {
            SaveToken().Wait();

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.Token))
            {
                return ApplicationData.Current.LocalSettings.Values[Constants.Token].ToString();
            }
            else
                return string.Empty;

        }

        private static async Task<string> SaveToken()
        {
            string token = await BaseClient.GetEntities($"setting/authorize?key={GetKey()}", "");

            ApplicationData.Current.LocalSettings.Values[Constants.Token] = token;

            return token;
        }
        public static Color GetColorFromHex(string colorStr)
        {

            //Target hex string
            colorStr = colorStr.Replace("#", string.Empty);
            // from #RRGGBB string
            byte r = (byte)System.Convert.ToUInt32(colorStr.Substring(0, 2), 16);
            byte g = (byte)System.Convert.ToUInt32(colorStr.Substring(2, 2), 16);
            byte b = (byte)System.Convert.ToUInt32(colorStr.Substring(4, 2), 16);
            //get the color
            Color color = Color.FromArgb(255, r, g, b);
            return color;
        }
        internal static bool IsInternet()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        internal static string GetKey()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(Constants.key))
            {
                return ApplicationData.Current.LocalSettings.Values[Constants.key].ToString();
            }
            else
                return SetKey();

        }

        internal static string SetKey()
        {
            string key = Guid.NewGuid().ToString();
            ApplicationData.Current.LocalSettings.Values[Constants.key] = key;

            return key;
        }
        public static string ToTitle(string s)
        {
            if (s == null || s.Length == 0)
            {
                return s;
            }

            string[] splits = s.Split(' ');

            for (int i = 0; i < splits.Length; i++)
            {
                switch (splits[i].Length)
                {
                    case 1:
                        break;

                    default:
                        splits[i] = char.ToUpper(splits[i][0]) + splits[i].Substring(1);
                        break;
                }
            }

            return string.Join(" ", splits);
        }
    }
}
