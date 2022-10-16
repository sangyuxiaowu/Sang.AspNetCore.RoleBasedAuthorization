using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorizationRolePermission
{
    /// <summary>
    /// 动态添加角色的 Permission
    /// </summary>
    internal class RolePermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RolePermissionOptions _opt;

        public RolePermissionMiddleware(RequestDelegate next, RolePermissionOptions opt)
        {
            _next = next;
            _opt = opt;
        }

        /// <summary>
        /// 自定义中间件要执行的逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {

            if(context.User is null)
            {
                await _next(context);
                return;
            }
            // 使用 Sang.AspNetCore.RoleBasedAuthorization 时，设置一个自定义角色，使其拥有 SangRBAC_Administrator 一样的系统内置超级管理员权限
            if (!string.IsNullOrWhiteSpace(_opt.userAdministratorRoleName) && _opt.userAdministratorRoleName != "SangRBAC_Administrator")
            {
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Role, "SangRBAC_Administrator"),
                };
                context.User.AddIdentity(new ClaimsIdentity(claims));
            }

            // 获取用户的所有角色
            var roles = context.User.FindAll(ClaimTypes.Role);
            // 逐个获取角色的 claims 并添加给 User
            foreach (var role in roles)
            {
                var roleclaims = await _opt.rolePermission.GetRolePermissionClaimsByName(role.Value);
                if (roleclaims != null)
                {
                    context.User.AddIdentity(new ClaimsIdentity(roleclaims));
                }
            }
            await _next(context);
        }
    }
}
