using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sang.AspNetCore.RoleBasedAuthorization.RolePermission
{
    public static class RolePermissionExtensions
    {
        /// <summary>
        /// 添加根据角色名为 User 加入角色 Permission 的中间件
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRolePermission(this IApplicationBuilder app, Action<RolePermissionOptions> configureOptions)
        {
            var options = new RolePermissionOptions();
            configureOptions(options);
            return app.UseMiddleware<RolePermissionMiddleware>(options);
        }

        /// <summary>
        /// 添加根据角色名为 User 加入角色 Permission 的中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRolePermission(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RolePermissionMiddleware>(new RolePermissionOptions());
        }

        /// <summary>
        /// 添加角色权限查询服务
        /// </summary>
        /// <typeparam name="RolePermission">获取角色权限的实现</typeparam>
        /// <param name="Services"></param>
        public static void AddRolePermission<RolePermission>(this IServiceCollection Services) where RolePermission : class, IRolePermission
        {
            Services.AddSingleton<IRolePermission, RolePermission>();
        }
    }
}