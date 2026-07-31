using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorization.RolePermission
{
    /// <summary>
    /// RolePermission 配置项
    /// </summary>
    public class RolePermissionOptions
    {
        /// <summary>
        /// 是否一直检查并执行添加，默认只有在含有 ResourceAttribute 要进行权限验证时，此次访问中间件才启动添加权限功能
        /// </summary>
        public bool Always { get; set; } = false;
    }
}
