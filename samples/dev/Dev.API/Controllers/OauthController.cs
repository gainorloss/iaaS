using Dev.API.Infras;
using Galosoft.IaaS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;

namespace Dev.API.Controllers
{
    /// <summary>
    /// 账号
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        private readonly JwtHelper _jwt;

        public OauthController(JwtHelper jwt)
        {
            _jwt = jwt;
        }

        [AllowAnonymous,HttpGet]
        [RT]
        public Result Auth()
        {
            var identity = new ClaimsIdentity(new Claim[]
              {
                    new Claim(ClaimTypes.Name, "administrator"),
                    new Claim("created_at",DateTime.Now.ToString())
              });

            var token = _jwt.GenerateToken(identity);
            return Result.Succeed(token);
        }

        [HttpGet]
        public Result Profile()
        {
            var claims = HttpContext.User.Claims.ToDictionary(c => c.Type, c => c.Value);
            return Result.Succeed(claims);
        }
    }
}
