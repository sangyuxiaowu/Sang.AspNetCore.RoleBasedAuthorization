using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorizationRolePermission
{
    /// <summary>
    /// RolePermission 配置项
    /// </summary>
    public class RolePermissionOptions
    {
        /// <summary>
        /// IRolePermission 的实现，根据角色名获取角色的 Permission
        /// </summary>
        public IRolePermission rolePermission { get; set; }

        /// <summary>
        /// 设置一个自定义角色，使其拥有 SangRBAC_Administrator 一样的系统内置超级管理员权限
        /// </summary>
        public string? userAdministratorRoleName { get; set; } = null;
    }
}
