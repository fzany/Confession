using Backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;

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
                string count = Store.CommentClass.GetCommentCount(guid);
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
                bool isSafe = Logic.CheckSpamFree(data.Comment.Body.ToLower());
                if (isSafe)
                {
                    Store.CommentClass.CreateComment(data);
                    Push.SendCommentNotification(data);
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Comment.Owner_Guid);
                }

                //send back the confession
                ConfessLoader loader = Store.ConfessClass.FetchOneConfessLoader(data.Comment.Confess_Guid, data.Comment.Owner_Guid);
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