using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpGet]
        [Route("chat/fetchrooms")]
        public ActionResult<List<ChatRoomLoader>> FetchChatRooms()
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                //prepare responses
                List<ChatRoomLoader> response = Store.ChatClass.FetchRooms(userKey);
                response = response.OrderBy(d => d.Title).ToList();
                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("chat/join")]
        public ActionResult Join(string id)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                //prepare responses
                Store.ChatClass.JoinRoom(userKey, id);
                //return data
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("chat/leave")]
        public ActionResult Leave(string id)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                //prepare responses
                Store.ChatClass.LeaveRoom(userKey, id);
                //return data
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet]
        [Route("chat/fetchchats")]
        public ActionResult<List<ChatLoader>> FetchChatsByRoom(string roomID)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                //prepare responses
                List<ChatLoader> response = Store.ChatClass.FetchChatByRooms(userKey, roomID);
                response = response.OrderBy(d => d.Date).ToList();
                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                // return StatusCode(500, ex.ToString());
                return new List<ChatLoader>() { };
            }
        }

        [HttpPost]
        [Route("chat/add")]
        public ActionResult<List<ChatLoader>> Add([FromBody]Chat data)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                List<ChatLoader> loader = Store.ChatClass.AddChat(data, userKey);
                return Ok(loader);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("chat/postimage")]
        public async Task<ActionResult<string>> PostImageAsync()
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                System.IO.Stream stream = Request.Body;
                if (stream == null)
                {
                    return BadRequest();
                }

                string ImageUrl = await Cloud.SaveChatImageAsync(stream);

                return Ok(ImageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete]
        [Route("chat/delete")]
        public ActionResult Delete(string id)
        {
            ClaimsIdentity claimsIdentity = this.User.Identity as ClaimsIdentity;
            string userKey = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            try
            {
                Store.ChatClass.DeleteChat(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}