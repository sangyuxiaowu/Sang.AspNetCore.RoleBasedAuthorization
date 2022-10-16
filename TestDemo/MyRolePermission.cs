using Sang.AspNetCore.RoleBasedAuthorizationRolePermission;
using System.Security.Claims;

namespace TestDemo
{
    public class MyRolePermission : IRolePermission
    {
        public Task<IEnumerable<Claim>?> GetRolePermissionClaimsByName(string roleName)
        {
            return null;
        }
    }
}
