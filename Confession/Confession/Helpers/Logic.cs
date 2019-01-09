using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Confession.Helpers
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

        public static string[] Categories = { "Love", "Sex", "Family", "Food", "Religion","Travel","General","Money", "Health" };
       
        public static string ToTitle(string input)
        {
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        static async Task <string> Createkey()
        {
            string key = Guid.NewGuid().ToString();
            try
            {
                await SecureStorage.SetAsync(Constants.key, key);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
            return key;
        }

        public static async Task<string> GetKey()
        {
            try
            {
                var oauthToken = await SecureStorage.GetAsync(Constants.key);
                if(oauthToken == null)
                {
                    return await Createkey();
                }
                return oauthToken;
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                return "";
            }
        }

       
    }
}
