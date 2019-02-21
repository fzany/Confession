using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared;
using Microsoft.AspNetCore.Mvc;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [HttpGet]
        [Route("comment/fetch/count")]
        public ActionResult<string> GetCommentCount(string guid)
        {
            try
            {
                var count = Store.CommentClass.GetCommentCount(guid);
              //return data
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
        [HttpGet]
        [Route("comment/fetch")]
        public ActionResult<List<CommentLoader>> FetchByConfessId(string guid, string key)
        {
            try
            {
                //prepare responses
                List<CommentLoader> response = Store.CommentClass.FetchComment(guid, key);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("comment/add")]
        public ActionResult<ConfessLoader> Add([FromBody]CommentPoster data)
        {
            try
            {
                ConfessLoader loader =Store.CommentClass.CreateComment(data);
                return Ok(loader);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete]
        [Route("comment/delete")]
        public ActionResult<ConfessLoader> Delete(string guid, string confess, string key)
        {
            try
            {
                Store.CommentClass.DeleteComment(guid);
                ConfessLoader loader = Store.ConfessClass.FetchOneConfessLoader(confess, key);
                return Ok(loader);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}