using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    public class ResourceClaimTypes
    {
        /// <summary>
        /// Sang RBAC 权限检查的 ClaimType 为 Permission
        /// </summary>
        public static readonly string Permission = "Permission";
    }

    public class ResourceRole
    {
        /// <summary>
        /// 系统内置超级管理员权限的角色名，该角色不进行权限检查
        /// </summary>
        public static readonly string Administrator = "SangRBAC_Administrator";
    }
}
