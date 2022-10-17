using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestDemo.Controllers
{

    /// <summary>
    /// 角色管理
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var endpoint = this.HttpContext.GetEndpoint();
            return Ok(MyRolePermission.MyRole);
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            return Ok(MyRolePermission.MyRole[name]);
        }

        /// <summary>
        /// 添加角色权限
        /// </summary>
        /// <param name="name">角色</param>
        /// <param name="value">权限</param>
        [HttpPost]
        public void Post([FromBody] AddRole add)
        {
            MyRolePermission.AddRole(add.name, add.value);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="name">角色</param>
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            MyRolePermission.MyRole.Remove(name);
        }
    }

    public record AddRole(string name, string value);
}
