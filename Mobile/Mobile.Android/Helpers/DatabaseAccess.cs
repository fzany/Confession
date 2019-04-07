using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mobile.Helpers.Local;
using Mobile.Helpers;
using System.IO;
using Mobile.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(DatabaseAccess))]
namespace Mobile.Droid.Helpers
{
    public class DatabaseAccess : IDataBaseAccess
    {
        public string DatabasePath()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Constants.OFFLINE_DATABASE_NAME);

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            return path;
        }
    }
}