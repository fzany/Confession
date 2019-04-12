using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class ConfessController : ControllerBase
    {
        private ChatHub context;
        public ConfessController(ChatHub hub)
        {
            context = hub;
        }
        [HttpGet]
        [Route("confess/fetchall")]
        public ActionResult<List<ConfessLoader>> FetchAll(string key)
        {
            try
            {
                //prepare responses
                List<ConfessLoader> response = Store.ConfessClass.FetchAllConfess(key);

                //test for emptiness
                if (response.Count() == 0)
                {
                    return Ok(new List<ConfessLoader>() { });
                }

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("confess/fetch")]
        public ActionResult<List<ConfessLoader>> FetchByKey(string key)
        {
            try
            {
                //prepare responses
                List<ConfessLoader> response = Store.ConfessClass.FetchMyConfessions(key);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("confess/fetch/cat")]
        public ActionResult<List<ConfessLoader>> FetchByCategory(string cat, string key)
        {
            try
            {
                //prepare responses
                List<ConfessLoader> response = Store.ConfessClass.FetchConfessByCategory(cat, key);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("confess/fetch/guid")]
        public ActionResult<Confess> FetchByGuid(string guid)
        {
            try
            {
                //prepare responses
                Confess response = Store.ConfessClass.FetchOneConfessByGuid(guid);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("confess/fetchloader/guid")]
        public ActionResult<ConfessLoader> FetchLoaderByGuid(string guid, string key)
        {
            try
            {
                //prepare responses
                ConfessLoader response = Store.ConfessClass.FetchOneConfessLoader(guid, key);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("confess/add")]
        public ActionResult Add([FromBody]Confess data)
        {
            try
            {
                bool isSafe = Logic.CheckSpamFree(data.Body.ToLower());
                if (isSafe)
                {
                    Store.ConfessClass.CreateConfess(data);
                    Push.PushToEveryone(data);
                    var forget =context.SendGeneric(data.Guid, data.Owner_Guid);
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Owner_Guid);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete]
        [Route("confess/delete")]
        public ActionResult Delete(string guid)
        {
            try
            {
                Store.ConfessClass.DeleteConfess(guid);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut]
        [Route("confess/update")]
        public ActionResult Update([FromBody]Confess data)
        {
            try
            {
                bool isSafe = Logic.CheckSpamFree(data.Body.ToLower());
                if (isSafe)
                {
                    Store.ConfessClass.UpdateConfess(data);
                }
                else
                {
                    Push.NotifyOwnerOFSpam(data.Owner_Guid);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}