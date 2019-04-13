using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private static readonly ChatHub context = new ChatHub();

        [HttpGet]
        [Route("user/fetch")]
        public ActionResult<UserData> FetchUser(string appcenter)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                //prepare response
                var response = Store.UserClass.FetchUser(userKey, appcenter);
                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("user/add")]
        public ActionResult Add([FromBody]UserData data)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                 Store.UserClass.AddUser(data);
                return Ok();
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut]
        [Route("user/update")]
        public ActionResult Update([FromBody]UserData data)
        {
            try
            {
                Store.UserClass.UpdateUser(data);
                return Ok();
            }
            catch (Exception ex)
            {
                var forget_error = context.Error(ex);
                return StatusCode(500, ex.ToString());
            }
        }

    }
}