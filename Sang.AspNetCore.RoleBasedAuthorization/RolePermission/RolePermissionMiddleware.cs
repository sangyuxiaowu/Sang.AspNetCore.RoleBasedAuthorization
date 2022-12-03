using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
namespace Sang.AspNetCore.RoleBasedAuthorization.RolePermission
{
    /// <summary>
    /// 动态添加角色的 Permission
    /// </summary>
    internal sealed class RolePermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RolePermissionOptions _opt;
        private readonly IRolePermission _rolePermission;

        public RolePermissionMiddleware(RequestDelegate next, RolePermissionOptions opt, IRolePermission rolePermission)
        {
            _next = next;
            _opt = opt;
            _rolePermission = rolePermission;
        }

        /// <summary>
        /// 自定义中间件要执行的逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (context.User is null)
            {
                await _next(context);
                return;
            }

            // 非全部添加权限
            if (!_opt.Always)
            {
                var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
                if (endpoint is null)
                {
                    await _next(context);
                    return;
                }
                var endpointMetaData = endpoint!.Metadata;
                bool hasResourceAttribute = endpointMetaData.Any(x => x is ResourceAttribute);
                if (!hasResourceAttribute)
                {
                    await _next(context);
                    return;
                }
            }

            // 使用 Sang.AspNetCore.RoleBasedAuthorization 时，设置一个自定义角色，使其拥有 SangRBAC_Administrator 一样的系统内置超级管理员权限
            if (!string.IsNullOrWhiteSpace(_opt.UserAdministratorRoleName) // 设置有超级管理员
                && _opt.UserAdministratorRoleName != ResourceRole.Administrator // 和内置的不一样
                && context.User.IsInRole(_opt.UserAdministratorRoleName) // 该用户拥有
                )
            {
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Role, ResourceRole.Administrator),
                };
                context.User.AddIdentity(new ClaimsIdentity(claims));
            }

            // 获取用户的所有角色
            var roles = context.User.FindAll(ClaimTypes.Role);
            // 逐个获取角色的 claims 并添加给 User
            foreach (var role in roles.ToList())
            {
                var roleclaims = await _rolePermission.GetRolePermissionClaimsByName(role.Value);
                if (roleclaims.Count() > 0)
                {
                    context.User.AddIdentity(new ClaimsIdentity(roleclaims));
                }
            }
            await _next(context);
        }
    }
}
