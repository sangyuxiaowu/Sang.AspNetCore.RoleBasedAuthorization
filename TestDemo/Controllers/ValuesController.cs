using Microsoft.AspNetCore.Mvc;
using Sang.AspNetCore.RoleBasedAuthorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestDemo.Controllers
{

    /// <summary>
    /// 权限测试
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        
        /// <summary>
        /// 查询数值
        /// </summary>
        /// <returns></returns>
        [Resource("查询", Action = "数值")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 查询数值信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Resource("查询-单个数值")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 新建-数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Resource("新建-数值")]
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok("新建-数值");
        }

        /// <summary>
        /// 更新-数值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Resource("更新-数值")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok("更新-数值");
        }

        /// <summary>
        /// 删除-数值
        /// </summary>
        /// <param name="id"></param>
        [Resource("删除-数值")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok("删除-数值");
        }
    }
}
