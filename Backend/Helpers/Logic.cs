using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Backend.Helpers
{
    public class Logic
    {
        public static AppSettings _appSettings;
        public Logic(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public static IEnumerable<SelectListItem> GetIEnumarablelist()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            List<string> cats = Categories.ToList();
            foreach (string dat in cats)
            {
                items.Add(new SelectListItem() { Text = dat, Value = dat });
            }
            return items;
        }
        internal static string GetGuid()
        {
            return MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        }
        public static string[] Categories = { "Love", "Sex", "Family",
            "Food", "Religion","Travel",
            "General","Money", "Health",
            "Crime" ,"Hilarious"};

        public const string Authkey = "jjjjjjjjjjjj12udiyd7dsiud68d7s8d6sds7dsfdsf67f67df7d6f7fd34567890!@#$%^&*(){:?><,/;'''''[]|";
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



        internal static string GetToken(string userkey)
        {
            //generate jwt token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes("jjjjjjjjjjjj12udiyd7dsiud68d7s8d6sds7dsfdsf67f67df7d6f7fd34567890!@#$%^&*(){:?><,/;'''''[]|");
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userkey)
                }),
                Expires = DateTime.UtcNow.AddYears(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            //fill data
            return tokenHandler.WriteToken(token);
        }

        internal static bool CheckSpamFree(string v)
        {
            foreach (string spam in Constants.SpamWords)
            {
                if (v.Contains(spam))
                    return false;
            }
            return true;
        }

        public static string ToTitle(string input)
        {
            return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }

        public static string GetException(Exception exc)
        {
            StringBuilder sw = new StringBuilder();
            sw.AppendLine($"********************, { DateTime.UtcNow}");
            if (exc.InnerException != null)
            {
                sw.Append("Inner Exception Type: ");
                sw.AppendLine(exc.InnerException.GetType().ToString());
                sw.Append("Inner Exception: ");
                sw.AppendLine(exc.InnerException.Message);
                sw.Append("Inner Source: ");
                sw.AppendLine(exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    sw.AppendLine("Inner Stack Trace: ");
                    sw.AppendLine(exc.InnerException.StackTrace);
                }
            }
            sw.Append("Exception Type: ");
            sw.AppendLine(exc.GetType().ToString());
            sw.AppendLine("Exception: " + exc.Message);
            sw.AppendLine("Source: ");
            sw.AppendLine("Stack Trace: ");
            if (exc.StackTrace != null)
            {
                sw.AppendLine(exc.StackTrace);
                sw.AppendLine();
            }
            sw.AppendLine(exc.ToString());

            return sw.ToString();
        }

    }
}
