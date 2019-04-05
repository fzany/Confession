using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class GenericController : ControllerBase
    {
        [HttpGet]
        [Route("generic/getaname")]
        public ActionResult<string> GetRandomName()
        {
            try
            {
                Random ran = new Random();
                int num = ran.Next(0, LargeFiles.Names.Count());
                string random_name = LargeFiles.Names[num];
                //return data
                return Ok(random_name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}