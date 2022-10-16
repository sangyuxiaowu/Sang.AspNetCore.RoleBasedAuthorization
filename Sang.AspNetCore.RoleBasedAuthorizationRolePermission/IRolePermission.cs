using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorizationRolePermission
{
    public interface IRolePermission
    {
        /// <summary>
        /// 获取角色的所有 Permission 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<IEnumerable<Claim>?> GetRolePermissionClaimsByName(string roleName);
    }
}
