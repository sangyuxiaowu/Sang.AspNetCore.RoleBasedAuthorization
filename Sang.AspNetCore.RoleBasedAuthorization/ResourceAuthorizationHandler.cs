using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    internal sealed class ResourceAuthorizationHandler : AuthorizationHandler<ResourceAttribute>
    {
        /// <summary>
        /// 授权处理
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="requirement">资源验证要求</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceAttribute requirement)
        {
            // 需要有用户
            if (context.User is null) return Task.CompletedTask;


            if (context.User.IsInRole(ResourceRole.Administrator) // 超级管理员权限，拥有 SangRBAC_Administrator 角色不检查权限
                || CheckClaims(context.User.Claims, requirement) // 符合 Resource 或 Resource-Action 组合的 Permission
                )
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }

        /// <summary>
        /// 检查 Claims 是否符合要求
        /// </summary>
        /// <param name="claims">待检查的claims</param>
        /// <param name="requirement">检查的依据</param>
        /// <returns></returns>
        private bool CheckClaims(IEnumerable<Claim> claims, ResourceAttribute requirement)
        {
            return claims.Any(c =>
                        string.Equals(c.Type, ResourceClaimTypes.Permission, StringComparison.OrdinalIgnoreCase)
                        && (string.Equals(c.Value, requirement.GetResource(), StringComparison.Ordinal)
                        || string.Equals(c.Value, $"{requirement.GetResource()}-{requirement.Action}", StringComparison.Ordinal))
                        );
        }
    }
}