using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class DislikeController : ControllerBase
    {
        private static readonly ChatHub context = new ChatHub();

        [HttpGet]
        [Route("dislike/fetch/count")]
        public ActionResult<string> GetCount(string guid, bool isComment)
        {
            try
            {
                string count = Store.DislikeClass.GetCount(guid, isComment);
                //return data
                return Ok(count);
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("dislike/fetch/exist")]
        public ActionResult<bool> CheckExistence(string guid, bool isComment, string key)
        {
            try
            {
                bool exist = Store.DislikeClass.CheckExistence(guid, isComment, key);
                //return data
                return Ok(exist);
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("dislike/add")]
        public ActionResult<ConfessSender> Post(string guid, bool isComment, string key, string confess)
        {
            try
            {
                bool exist = Store.DislikeClass.Post(guid, isComment, key);
                ConfessSender sender = new ConfessSender() { Loader = Store.ConfessClass.FetchOneConfessLoader(confess, key)
                , IsSuccessful = exist};

                //return data
                return Ok(sender);
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}