using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sang.AspNetCore.RoleBasedAuthorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestDemo.Controllers
{

    /// <summary>
    /// 测试RBAC
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SangPermissionController : ControllerBase
    {

        private readonly IOptionsSnapshot<JWTSettings> _jwtSettings;
        private readonly ILogger<SangPermissionController> _logger;
        private readonly IPermissionLocalizer _permissionLocalizer;

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="jwtSettings"></param>
        /// <param name="permissionLocalizer">权限文案本地化服务。</param>
        public SangPermissionController(
            ILogger<SangPermissionController> logger,
            IOptionsSnapshot<JWTSettings> jwtSettings,
            IPermissionLocalizer permissionLocalizer)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
            _permissionLocalizer = permissionLocalizer;
        }

        /// <summary>
        /// 全部权限列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Resources")]
        public IActionResult Resources([FromQuery] string? culture = null)
        {
            var requestedCulture = culture ?? Request.Headers.AcceptLanguage.FirstOrDefault()?.Split(',', 2)[0];
            var resolvedCulture = _permissionLocalizer.ResolveCulture(requestedCulture);
            return Ok(ResourceData.GetResourceInfos().Localize(_permissionLocalizer, resolvedCulture));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="roleName">登录用户的角色</param>
        /// <returns></returns>
        [HttpGet("login")]
        public IActionResult CheckPassword(string? roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "uid"),
                new Claim(ClaimTypes.Name,"用户名"),
                new Claim(ClaimTypes.Email,"test@exp.com"),
                new Claim(ClaimTypes.Role, "user"),
                //new Claim(ResourceClaimTypes.Permission,"查询"),
            };
            // 添加角色名
            if (!string.IsNullOrWhiteSpace(roleName)) claims.Add(new Claim(ClaimTypes.Role, roleName));
            var token = new JwtSecurityToken(
                    "Issuer",
                    "Audience",
                    claims,
                    expires: DateTime.UtcNow.AddSeconds(_jwtSettings.Value.ExpireSeconds),
                    signingCredentials: credentials
                );

            return Ok(new
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        [Authorize]
        public IActionResult GetUser()
        {
            Dictionary<string, string> list = new();
            int i = 0;
            foreach(var c in this.User.Claims)
            {
                i++;
                list.Add($"{i} - {c.Type}", c.Value);
            }
            return Ok(list);
        }

        /// <summary>
        /// 查询天气
        /// </summary>
        /// <returns></returns>
        [Resource("weather", "read", "天气", "查看天气", "允许查看天气预报")]
        [HttpGet("getweather")]
        public IActionResult Get()
        {
            return Ok("查询-天气");
        }


        
    }
}