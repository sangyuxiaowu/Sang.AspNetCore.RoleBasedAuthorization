using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    internal class ResourceAuthorizationHandler : AuthorizationHandler<ResourceAuthorizationRequirement>
    {
        /// <summary>
        /// 授权处理
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <param name="requirement">资源验证要求</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceAuthorizationRequirement requirement)
        {
            // 需要有用户
            if (context.User is null) return Task.CompletedTask;

            // Resource 或 Resource-Action 组合
            if(CheckClaims(context.User.Claims, requirement))
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
        private bool CheckClaims(IEnumerable<Claim> claims, ResourceAuthorizationRequirement requirement)
        {
            return claims.Any(c =>
                        string.Equals(c.Type, ResourceClaimTypes.Permission, StringComparison.OrdinalIgnoreCase)
                        && (string.Equals(c.Value, requirement.Resource, StringComparison.Ordinal)
                        || string.Equals(c.Value, $"{requirement.Resource}-{requirement.Action}", StringComparison.Ordinal))
                        );
        }
    }
}