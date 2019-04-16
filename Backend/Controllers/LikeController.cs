using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;

namespace Backend.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        


        [HttpGet]
        [Route("like/fetch/count")]
        public ActionResult<string> GetCount(string guid, bool isComment)
        {
            try
            {
                string count = Store.LikeClass.GetCount(guid, isComment);
                //return data
                return Ok(count);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("like/fetch/exist")]
        public ActionResult<bool> CheckExistence(string guid, bool isComment, string key)
        {
            try
            {
                bool exist = Store.LikeClass.CheckExistence(guid, isComment, key);
                //return data
                return Ok(exist);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("like/add")]
        public ActionResult<ConfessSender> Post(string guid, bool isComment, string key, string confess)
        {
            try
            {
                bool exist = Store.LikeClass.Post(guid, isComment, key);
                ConfessSender sender = new ConfessSender() { Loader = Store.ConfessClass.FetchOneConfessLoader(confess, key), IsSuccessful = exist };

                //return data
                return Ok(sender);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, ex.ToString());
            }
        }
    }
}