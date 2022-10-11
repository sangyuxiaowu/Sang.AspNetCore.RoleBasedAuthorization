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
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly IOptionsSnapshot<JWTSettings> _jwtSettings;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsSnapshot<JWTSettings> jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        [Resource("查询",Action ="查询天气")]
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("login")]
        public IActionResult CheckPassword()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "uid"),
                new Claim(ClaimTypes.Name,"用户名"),
                new Claim(ClaimTypes.Email,"test@exp.com"),
            };
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

        [HttpGet("Resources")]
        public IActionResult Resources()
        {
            return Ok(ResourceData.Resources);
        }
    }
}