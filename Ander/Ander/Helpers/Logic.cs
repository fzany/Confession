using Ander.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Ander.Helpers
{
    public class Logic
    {
        public static bool IsThisDevice()
        {
            // Device Model (SMG-950U, iPhone10,6)
            string device = DeviceInfo.Model;

            // Manufacturer (Samsung)
            string manufacturer = DeviceInfo.Manufacturer;

            // Device Name (Motz's iPhone)
            string deviceName = DeviceInfo.Name;

            // Operating System Version Number (7.0)
            string version = DeviceInfo.VersionString;

            // Platform (Android)
            DevicePlatform platform = DeviceInfo.Platform;

            // Idiom (Phone)
            DeviceIdiom idiom = DeviceInfo.Idiom;

            // Device Type (Physical)
            DeviceType deviceType = DeviceInfo.DeviceType;

            return true;
        }
        public static string DeviceInformation()
        {
            StringBuilder sob = new StringBuilder();
            // Device Model (SMG-950U, iPhone10,6)
            //string device = DeviceInfo.Model;
            sob.AppendLine($"Device Model: {DeviceInfo.Model}");

            // Manufacturer (Samsung)
            //string manufacturer = DeviceInfo.Manufacturer;
            sob.AppendLine($"Device Manufacturer: {DeviceInfo.Manufacturer}");


            // Device Name (Motz's iPhone)
            // string deviceName = DeviceInfo.Name;

            // Operating System Version Number (7.0)
            //string version = DeviceInfo.VersionString;
            sob.AppendLine($"Operating System Version Number: {DeviceInfo.VersionString}");

            // Platform (Android)
            //DevicePlatform platform = DeviceInfo.Platform;
            sob.AppendLine($"Device Platform: {DeviceInfo.Platform}");


            // Idiom (Phone)
            // DeviceIdiom idiom = DeviceInfo.Idiom;
            sob.AppendLine($"Device Idiom: {DeviceInfo.Idiom}");


            // Device Type (Physical)
            // DeviceType deviceType = DeviceInfo.DeviceType;
            sob.AppendLine($"Device Type: {DeviceInfo.DeviceType}");
            return sob.ToString();
        }


        public static string[] Categories = { "Love", "Sex", "Family",
            "Food", "Religion","Travel",
            "General","Money", "Health",
            "Crime" ,"Hilarious"};

        public static List<MasterItem> Masterlogos()
        {
            return new List<MasterItem>() {
                new MasterItem{ Icon =Constants.FontAwe.Heart, Title= Categories[0] },
                 new MasterItem{ Icon =Constants.FontAwe.Bed, Title= Categories[1] },
                new MasterItem{ Icon =Constants.FontAwe.Users, Title= Categories[2] },
                new MasterItem{ Icon =Constants.FontAwe.Utensils, Title= Categories[3] },
                new MasterItem{ Icon =Constants.FontAwe.Church, Title= Categories[4] },
                new MasterItem{ Icon =Constants.FontAwe.Plane, Title= Categories[5] },
                new MasterItem{ Icon =Constants.FontAwe.Circle, Title= Categories[6] },
                new MasterItem{ Icon =Constants.FontAwe.Piggy_bank, Title= Categories[7] },
                new MasterItem{ Icon =Constants.FontAwe.First_aid, Title= Categories[8] },
                new MasterItem{ Icon =Constants.FontAwe.Fire, Title= Categories[9] },
                new MasterItem{ Icon =Constants.FontAwe.Meh, Title= Categories[10] },

            };
        }

        public static string ToTitle(string input)
        {
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        private static async Task<string> Createkey()
        {
            string key = Guid.NewGuid().ToString();
            try
            {
                await SecureStorage.SetAsync(Constants.key, key);
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return key;
        }

        public static async Task<string> GetKey()
        {
            try
            {
                string oauthToken = await SecureStorage.GetAsync(Constants.key);
                if (oauthToken == null)
                {
                    return await Createkey();
                }
                return oauthToken;
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
                return "";
            }
        }
        public static async Task<string> GetToken()
        {
            try
            {
                string oauthToken = await SecureStorage.GetAsync(Constants.Token);
                if (oauthToken == null || string.IsNullOrEmpty(oauthToken))
                {
                    return await Createtoken();
                }
                return oauthToken;
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
                return "";
            }
        }

        private static async Task<string> Createtoken()
        {
            string token = await BaseClient.GetEntities($"setting/authorize?key={await GetKey()}", "");
            try
            {
                await SecureStorage.SetAsync(Constants.Token, JsonConvert.DeserializeObject<string>(token));
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return token;
        }
        public static ConfessLoader ProcessConfessLoader(ConfessLoader loader)
        {
            if (!string.IsNullOrEmpty(loader.LikeColorString))
            {
                loader.LikeColor = Color.FromHex(loader.LikeColorString);
            }

            if (!string.IsNullOrEmpty(loader.DislikeColorString))
            {
                loader.DislikeColor = Color.FromHex(loader.DislikeColorString);
            }
            return loader;
        }

        public static CommentLoader ProcessCommentLoader(CommentLoader loader)
        {
            if (!string.IsNullOrEmpty(loader.LikeColorString))
            {
                loader.LikeColor = Color.FromHex(loader.LikeColorString);
            }

            if (!string.IsNullOrEmpty(loader.DislikeColorString))
            {
                loader.DislikeColor = Color.FromHex(loader.DislikeColorString);
            }
            return loader;
        }
        public static bool IsInternet()
        {
            NetworkAccess current = Connectivity.NetworkAccess;
            return current == NetworkAccess.Internet;
        }

    }
}
