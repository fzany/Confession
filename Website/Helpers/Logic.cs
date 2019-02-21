using Microsoft.AspNetCore.Mvc.Rendering;
using Shared;
using System.Collections.Generic;

namespace Website.Helpers
{
    public class Logic
    {

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
        public static IEnumerable<SelectListItem> GetCategoriesLists()
        {
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (string item in Categories)
            {
                selectLists.Add(new SelectListItem() { Text = item, Value = item });
            }
            return selectLists;
        }

        public static IEnumerable<SelectListItem> GetCategoriesLists(string category)
        {
            List<SelectListItem> selectLists = new List<SelectListItem>();
            foreach (string item in Categories)
            {
                selectLists.Add(new SelectListItem() { Text = item, Value = item, Selected = (item == category) });
            }
            return selectLists;
        }

        public static string ToTitle(string input)
        {
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }


    }
}
