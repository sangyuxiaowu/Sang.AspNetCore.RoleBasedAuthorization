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
        /// 设置一个自定义角色，使其拥有 SangRBAC_Administrator 一样的系统内置超级管理员权限
        /// </summary>
        public string? UserAdministratorRoleName { get; set; } = null;

        /// <summary>
        /// 是否一直检查并执行添加，默认只有在含有 ResourceAttribute 要进行权限验证时，此次访问中间件才启动添加权限功能
        /// </summary>
        public bool Always { get; set; } = false;
    }
}
