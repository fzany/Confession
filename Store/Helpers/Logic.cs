using Newtonsoft.Json;
using Store.Helpers;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
        public static Windows.UI.Xaml.Media.Brush GetColorFromHex(string colorStr)
        {
            colorStr = colorStr.Replace("#", "");
            if (colorStr.Length == 6)
            {
                return new SolidColorBrush(ColorHelper.FromArgb(255,
                    byte.Parse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    byte.Parse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)));
            }
            else
            {
                return new SolidColorBrush(Colors.Black);
            }
        
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
