using Backend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Website.Helpers;
using Website.Models;
using Constants = Shared.Constants;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ConfessController confessController = new ConfessController();
        private string GetCookie(string key)
        {
            //read cookie from Request object  
            return Request.Cookies[key] ?? string.Empty;
        }
        private void CreateCookie(string key, string value)
        {
            CookieOptions option = new CookieOptions
            {
                Expires = DateTime.Now.AddYears(99)
            };
            Response.Cookies.Append(key, value, option);
        }
        //CurrentConfessGuid
        private void SetCurrent(string guid)
        {
            CreateCookie(Constants.CurrentConfessGuid, guid);
        }
        private string GetCurrent()
        {
            return GetCookie(Constants.CurrentConfessGuid);
        }
        private string GetKey()
        {
            string getKey = GetCookie(Constants.key);
            if (string.IsNullOrEmpty(getKey))
            {
                //create the key
                string newkey = Guid.NewGuid().ToString().Replace("-", "");
                CreateCookie(Constants.key, newkey);
                return newkey;
            }
            return getKey;
        }

        public string GetToken(string key)
        {
            try
            {
                string oauthToken = GetCookie(Helpers.Constants.Token);
                if (oauthToken == null || string.IsNullOrEmpty(oauthToken))
                {
                    //oauthToken = JsonConvert.DeserializeObject<string>(await Helpers.BaseClient.GetEntities($"setting/authorize?key={key}", "tester"));
                    oauthToken = Guid.NewGuid().ToString().Replace("-", "");
                    CreateCookie(Helpers.Constants.Token, oauthToken);
                    return oauthToken;
                }
                return oauthToken;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public PartialViewResult Menu()
        {
            Modeller modeller = new Modeller
            {
                Categories = Logic.Categories.ToList(),
                ListItems = Logic.GetCategoriesLists()
            };
            return PartialView(modeller);
        }
        public async Task<IActionResult> Index()
        {
            string id = Request.Path.Value.Split('/').LastOrDefault();

            //get the key
            string key = GetKey();
            string token = GetToken(key);

            //ActionResult<List<ConfessLoader>> okResult = confessController.FetchAll(GetKey());
            //List<ConfessLoader> results = (List<ConfessLoader>)((OkObjectResult)okResult.Result).Value;

            List<ConfessLoader> results = new List<ConfessLoader>();
            if (string.IsNullOrEmpty(id))
            {
                results = await Store.ConfessClass.FetchAllConfess(token, key);
            }
            else
            {
                if (id == "mine")
                {
                    results = await Store.ConfessClass.FetchMyConfessions(token, key);
                }
                else
                {
                    if (Logic.Categories.Contains(id))
                    {
                        results = await Store.ConfessClass.FetchConfessByCategory(id, token, key);
                    }
                    else
                    {
                        results = await Store.ConfessClass.FetchAllConfess(token, key);
                    }
                }
            }
            results.Reverse();
            Modeller modeller = new Modeller
            {
                ConfessLoaders = results,
                Categories = Logic.Categories.ToList(),
                ListItems = Logic.GetCategoriesLists()
            };


            //var list = 

            //Log user visit

            return View(modeller);
        }

        public async Task<IActionResult> Add(Modeller confessed)
        {
            try
            {
                Confess confess = confessed.Confess;// new Confess();// (Confess)confessed;
                if (string.IsNullOrWhiteSpace(confess.Title) ||
                    string.IsNullOrWhiteSpace(confess.Body) ||
                    string.IsNullOrWhiteSpace(confess.Category))
                {
                    return RedirectToActionPermanent("Index", "Home");
                }

                confess.Owner_Guid = GetKey();
                //Save
                await Store.ConfessClass.CreateConfess(confess, GetToken(GetKey()));
                return RedirectToActionPermanent("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToActionPermanent("Index", "Home");
            }
        }

        public async Task<IActionResult> Detail()
        {
            string id = Request.Path.Value.Split('/').LastOrDefault();
            SetCurrent(id);
            try
            {
                //Confess confess = await Store.ConfessClass.FetchOneConfessByGuid(id, GetToken(GetKey()));
                ConfessLoader loader = await Store.ConfessClass.FetchOneConfessLoaderByGuid(id, GetToken(GetKey()), GetKey());
                List<CommentLoader> loaders = await Store.CommentClass.FetchComment(loader.Guid, GetToken(GetKey()), GetKey());

                loaders.Reverse();
                ViewBag.key = GetKey();
                Modeller modeller = new Modeller()
                {
                    ConfessLoader = loader,
                    Loaders = loaders
                };
                return View(modeller);
                // return RedirectToAction("Detail", "Home", modeller);
            }
            catch (Exception)
            {
                return RedirectToActionPermanent("Index", "Home");
            }
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        //public IActionResult Edit()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        public async Task<IActionResult> Edit()
        {
            string guid = Request.Path.Value.Split('/').LastOrDefault();
            if (!string.IsNullOrEmpty(guid))
            {
                Confess confess = await Store.ConfessClass.FetchOneConfessByGuid(guid, GetToken(GetKey()));
                Modeller modeller = new Modeller() { Confess = confess, ListItems = Logic.GetCategoriesLists(confess.Category) };
                return View(modeller);
            }
            return View();
        }

        public async Task<IActionResult> Update(Modeller confessed)
        {
            Confess confess = confessed.Confess;
            try
            {
                if (string.IsNullOrWhiteSpace(confess.Title) ||
                    string.IsNullOrWhiteSpace(confess.Body) ||
                    string.IsNullOrWhiteSpace(confess.Category))
                {
                    return RedirectToActionPermanent($"Edit/{confess.Guid}", "Home");
                }
                Confess fetch = await Store.ConfessClass.FetchOneConfessByGuid(confess.Guid, GetToken(GetKey()));
                fetch.Title = confess.Title;
                fetch.Body = confess.Body;
                fetch.Category = confess.Category;

                //Save
                await Store.ConfessClass.UpdateConfess(fetch, GetToken(GetKey()));

                return RedirectToActionPermanent($"Edit/{confess.Guid}", "Home");
            }
            catch (Exception)
            {
                return RedirectToActionPermanent($"Edit/{confess.Guid}", "Home");
            }
        }
        //public async Task<IActionResult> Delete()
        //{
        //    string id = Request.Path.Value.Split('/').LastOrDefault();
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        await Store.ConfessClass.DeleteConfess(id, GetToken(GetKey()));
        //    }
        //    return RedirectToActionPermanent("Index", "Home");
        //}

        [HttpGet]
        public async Task<IActionResult> Liker(string guid, string ownerguid, string parentguid, bool iscomment)
        {
            if (ownerguid != GetKey())
            {
                await Store.LikeClass.Post(guid, iscomment, parentguid, GetToken(GetKey()), GetKey());
            }
            return RedirectToActionPermanent($"Detail/{parentguid}", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DisLiker(string guid, string ownerguid, string parentguid, bool iscomment)
        {
            if (ownerguid != GetKey())
            {
                await Store.DislikeClass.Post(guid, iscomment, parentguid, GetToken(GetKey()), GetKey());
            }
            return RedirectToActionPermanent($"Detail/{parentguid}", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(string guid, string parentguid)
        {
            await Store.CommentClass.DeleteComment(guid, parentguid, GetToken(GetKey()), GetKey());
            return RedirectToActionPermanent($"Detail/{parentguid}", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Detail(Modeller confessed)
        {
            Comment newComment = confessed.Comment;
            newComment.Owner_Guid = GetKey();
            newComment.Confess_Guid = GetCurrent();
            if (!string.IsNullOrWhiteSpace(newComment.Body))
            {
                await Store.CommentClass.CreateComment(newComment, newComment.Confess_Guid, GetToken(GetKey()), GetKey());
            }

            return RedirectToActionPermanent($"Detail/{newComment.Confess_Guid}", "Home");
            //return View("Detail");
           // return View();
        }

        public async Task<IActionResult> Delete(Modeller confessed)
        {
            string id = string.Empty;
            if (confessed.ConfessLoader != null)
            {
                id = confessed.ConfessLoader.Guid;
            }
            else if (confessed.Confess != null)
            {
                id = confessed.Confess.Guid;
            }

            if (!string.IsNullOrEmpty(id))
            {
                await Store.ConfessClass.DeleteConfess(id, GetToken(GetKey()));
            }
            return RedirectToActionPermanent("Index", "Home");
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ChatPage()
        {
           return View();
         //return RedirectToActionPermanent("ChatPage", "Home");
        }
    }
}
