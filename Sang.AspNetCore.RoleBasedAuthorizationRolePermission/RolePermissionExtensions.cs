using Microsoft.AspNetCore.Builder;

namespace Sang.AspNetCore.RoleBasedAuthorizationRolePermission
{
    public static class RolePermissionExtensions
    {
        /// <summary>
        /// 添加根据角色名为 User 加入角色 Permission 的中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRolePermission(this IApplicationBuilder app, Action<RolePermissionOptions> configureOptions)
        {
            var options = new RolePermissionOptions();
            configureOptions(options);
            return app.UseMiddleware<RolePermissionMiddleware>(options);
        }

    }
}