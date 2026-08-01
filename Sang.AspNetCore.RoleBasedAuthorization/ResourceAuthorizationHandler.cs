using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    internal sealed class ResourceAuthorizationHandler : AuthorizationHandler<ResourceAttribute>
    {
        private readonly SangRoleBasedAuthorizationOptions _options;

        /// <summary>
        /// 构造资源授权处理程序
        /// </summary>
        /// <param name="options">Sang RBAC 配置项</param>
        public ResourceAuthorizationHandler(IOptions<SangRoleBasedAuthorizationOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

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


            if (context.User.IsInRole(_options.AdministratorRoleName) // 超级管理员权限，拥有配置的管理员角色名时不检查权限
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
            var resource = requirement.Resource!;

            return claims.Any(c =>
                        string.Equals(c.Type, ResourceClaimTypes.Permission, StringComparison.OrdinalIgnoreCase)
                        && (
                            string.Equals(c.Value, "*", StringComparison.Ordinal) // 全局通配，拥有全部权限
                            || string.Equals(c.Value, resource, StringComparison.Ordinal) // 资源级授权，拥有该资源全部操作
                            || string.Equals(c.Value, $"{resource}.*", StringComparison.Ordinal) // 资源操作通配
                            || string.Equals(c.Value, requirement.Permission, StringComparison.Ordinal) // 精确匹配资源-操作
                        )
                        );
        }
    }
}