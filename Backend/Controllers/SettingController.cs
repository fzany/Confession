using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared;

namespace Backend.Controllers
{
   // [Authorize]
    [Produces("application/json")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        [HttpGet]
        [Route("setting/fetch")]
        public ActionResult<Setting> GetSetting()
        {
            try
            {
                Setting setting = new Setting {
                    Categories = Store.SettingsClass.GetCategories(),
                      MasterItems = Logic.Masterlogos()
                };
                //return data
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("setting/authorize")]
        public ActionResult<string> GetSetting(string key)
        {
            try
            {
                //fill data
                string response = Logic.GetToken(key);

                //return data
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [Route("settings/log")]
        public ActionResult Add([FromBody]DeviceInfo data)
        {
            try
            {
                Store.SettingsClass.CreateLog(data);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}