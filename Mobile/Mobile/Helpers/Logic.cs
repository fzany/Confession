using Microsoft.AppCenter.Crashes;
using Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mobile.Helpers
{
    public class Logic
    {
        public static bool IsThisDevice()
        {
            // Device Model (SMG-950U, iPhone10,6)
            string device = Xamarin.Essentials.DeviceInfo.Model;

            // Manufacturer (Samsung)
            string manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer;

            // Device Name (Motz's iPhone)
            string deviceName = Xamarin.Essentials.DeviceInfo.Name;

            // Operating System Version Number (7.0)
            string version = Xamarin.Essentials.DeviceInfo.VersionString;

            // Platform (Android)
            DevicePlatform platform = Xamarin.Essentials.DeviceInfo.Platform;

            // Idiom (Phone)
            DeviceIdiom idiom = Xamarin.Essentials.DeviceInfo.Idiom;

            // Device Type (Physical)
            DeviceType deviceType = Xamarin.Essentials.DeviceInfo.DeviceType;


            return true;
        }

        public static Models.DeviceInfo GetDeviceInformation()
        {
            Models.DeviceInfo logger = new Models.DeviceInfo
            {
                Model = Xamarin.Essentials.DeviceInfo.Model,
                Manufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer,
                DeviceType = Xamarin.Essentials.DeviceInfo.DeviceType.ToString(),
                Name = Xamarin.Essentials.DeviceInfo.Name,
                Idiom = Xamarin.Essentials.DeviceInfo.Idiom.ToString(),
                Platform = Xamarin.Essentials.DeviceInfo.Platform.ToString(),
                VersionString = Xamarin.Essentials.DeviceInfo.VersionString
            };

            return logger;
        }


        public static string DeviceInformation()
        {
            StringBuilder sob = new StringBuilder();
            // Device Model (SMG-950U, iPhone10,6)
            //string device = DeviceInfo.Model;
            sob.AppendLine($"Device Model: {Xamarin.Essentials.DeviceInfo.Model}");

            // Manufacturer (Samsung)
            //string manufacturer = DeviceInfo.Manufacturer;
            sob.AppendLine($"Device Manufacturer: {Xamarin.Essentials.DeviceInfo.Manufacturer}");


            // Device Name (Motz's iPhone)
            // string deviceName = DeviceInfo.Name;

            // Operating System Version Number (7.0)
            //string version = DeviceInfo.VersionString;
            sob.AppendLine($"Operating System Version Number: {Xamarin.Essentials.DeviceInfo.VersionString}");

            // Platform (Android)
            //DevicePlatform platform = DeviceInfo.Platform;
            sob.AppendLine($"Device Platform: {Xamarin.Essentials.DeviceInfo.Platform}");


            // Idiom (Phone)
            // DeviceIdiom idiom = DeviceInfo.Idiom;
            sob.AppendLine($"Device Idiom: {Xamarin.Essentials.DeviceInfo.Idiom}");


            // Device Type (Physical)
            // DeviceType deviceType = DeviceInfo.DeviceType;
            sob.AppendLine($"Device Type: {Xamarin.Essentials.DeviceInfo.DeviceType}");

            return sob.ToString();
        }

        internal static async Task<ObservableCollection<MasterItem>> SetCategories()
        {
            ObservableCollection<MasterItem> MenuItems = new ObservableCollection<MasterItem>() { };
            List<string> cat = (await Store.SettingClass.GetSettings()).Categories.ToList();
            List<MasterItem> cat_logos = Logic.Masterlogos();
            cat.Sort();
            foreach (string dt in cat)
            {
                MenuItems.Add(new MasterItem() { Title = dt, Icon = cat_logos.FirstOrDefault(d => d.Title == dt).Icon });
            }
            return MenuItems;
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

        public static async Task<string> Createkey()
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

        public static async Task<string> CreateroomID(string roomID)
        {
            try
            {
                await SecureStorage.SetAsync(Constants.roomID, roomID);
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return roomID;
        }
        public static async Task<string> GetRoomID()
        {
            try
            {
                string oauthToken = await SecureStorage.GetAsync(Constants.roomID);
                return oauthToken;
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
                return "";
            }
        }

        public static async Task<string> CreateChatName(string name)
        {
            string chatname = Logic.ToTitle(name);
            try
            {
                await SecureStorage.SetAsync(Constants.chatname, chatname);
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return chatname;
        }
        public static async Task<string> GetChatName()
        {
            try
            {
                string oauthToken = await SecureStorage.GetAsync(Constants.chatname);
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

        public static async Task<string> Createtoken()
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

        public static async Task<string> CheckUserRegistration()
        {
            try
            {
                string oauthLogged = await SecureStorage.GetAsync(Constants.UserRegistered);
                if (oauthLogged == null)
                {
                    return string.Empty;
                }
                return oauthLogged;
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
                return "";
            }
        }

        public static async Task<string> CreateLogged()
        {
            string Logged = "true";//;
            try
            {
                await SecureStorage.SetAsync(Constants.UserRegistered, Logged);
            }
            catch (Exception)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return Logged;
        }
        public static void VibrateNow()
        {
            try
            {
                // Or use specified time
                TimeSpan duration = TimeSpan.FromMilliseconds(10);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
                Crashes.TrackError(ex);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public static Color GetColourFromName(string name)
        {
            List<string> colours = new List<string>() { "#f25022", "#7fba00", "#00a4ef", "#ffb900", "#737373", "#4285F4", "#DB4437", "#F4B400", "#0F9D58" };

            if (string.IsNullOrEmpty(name))
            {
                return Color.FromHex(colours[0]);
            }

            int sum = 0;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i]; // declare it in loop - you overwrite it here anyway
                sum += char.ToUpper(c) - 64;
            }

            int index = sum % 9;
            return Color.FromHex(colours[index]);

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
