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

            var permissionClaims = new List<Claim>();

            // 获取用户的角色，并根据角色获取权限声明
            foreach (var role in context.User.FindAll(ClaimTypes.Role))
            {
                permissionClaims.AddRange(await _rolePermission.GetRolePermissionClaimsByName(role.Value));
            }

            // 获取用户直接授予的权限声明
            permissionClaims.AddRange(await _rolePermission.GetUserPermissionClaims(context.User));

            // 移除重复的权限声明，确保每个权限声明只出现一次
            var distinctClaims = permissionClaims
                .GroupBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase)
                .SelectMany(group => group.GroupBy(claim => claim.Value, StringComparer.Ordinal))
                .Select(group => group.First());

            if (distinctClaims.Any())
            {
                context.User.AddIdentity(new ClaimsIdentity(distinctClaims));
            }
            await _next(context);
        }
    }
}
