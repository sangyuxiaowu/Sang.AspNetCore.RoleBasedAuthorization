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
    /// ����RBAC
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SangPermissionController : ControllerBase
    {

        private readonly IOptionsSnapshot<JWTSettings> _jwtSettings;
        private readonly ILogger<SangPermissionController> _logger;

        /// <summary>
        /// ��ѯ
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="jwtSettings"></param>
        public SangPermissionController(ILogger<SangPermissionController> logger, IOptionsSnapshot<JWTSettings> jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// ȫ��Ȩ���б�
        /// </summary>
        /// <returns></returns>
        [HttpGet("Resources")]
        public IActionResult Resources()
        {
            return Ok(ResourceData.Resources);
        }

        /// <summary>
        /// �û���¼
        /// </summary>
        /// <param name="roleName">��¼�û��Ľ�ɫ</param>
        /// <returns></returns>
        [HttpGet("login")]
        public IActionResult CheckPassword(string? roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "uid"),
                new Claim(ClaimTypes.Name,"�û���"),
                new Claim(ClaimTypes.Email,"test@exp.com"),
                new Claim(ClaimTypes.Role, "user"),
                new Claim(ResourceClaimTypes.Permission,"��ѯ"),
            };
            // ��ӽ�ɫ��
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
        /// �û���Ϣ
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
        /// ��ѯ����
        /// </summary>
        /// <returns></returns>
        [Resource("��ѯ",Action ="����")]
        [HttpGet("getweather")]
        public IActionResult Get()
        {
            return Ok("��ѯ-����");
        }


        
    }
}