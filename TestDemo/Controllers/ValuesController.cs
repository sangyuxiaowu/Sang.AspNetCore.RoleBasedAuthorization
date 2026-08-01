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
    [ResourceModule("values", "数值")]
    public class ValuesController : ControllerBase
    {
        
        /// <summary>
        /// 查询数值
        /// </summary>
        /// <returns></returns>
        [Resource("read", "查看数值", "允许查看数值列表")]
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
        [Resource("read-detail", "查看数值详情", "允许查看单个数值")]
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
        [Resource("create", "新建数值", "允许新建数值")]
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok("数值.新建");
        }

        /// <summary>
        /// 更新-数值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Resource("update", "更新数值", "允许更新数值")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok("数值.更新");
        }

        /// <summary>
        /// 删除-数值
        /// </summary>
        /// <param name="id"></param>
        [Resource("delete", "删除数值", "允许删除数值")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok("数值.删除");
        }
    }
}
