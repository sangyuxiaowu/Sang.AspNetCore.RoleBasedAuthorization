using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 提供 Sang 基于资源的授权服务注册扩展。
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// 添加 Sang.AspNetCore.RoleBasedAuthorization 服务
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void AddSangRoleBasedAuthorization(this IServiceCollection services)
        {
            services.AddSangRoleBasedAuthorization(_ => { });
        }

        /// <summary>
        /// 添加 Sang.AspNetCore.RoleBasedAuthorization 服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">配置回调</param>
        public static void AddSangRoleBasedAuthorization(this IServiceCollection services, Action<SangRoleBasedAuthorizationOptions> configure)
        {
            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure(configure);

            // 注册定义的 IApplicationModelProvider 用于获取到全局定义的 Resource 特性的信息
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ResourceInfoModelProvider>());
            // 添加自定义授权策略
            services.AddSingleton<IAuthorizationPolicyProvider, ResourceAuthorizationPolicyProvider>();
            // 添加自定义授权处理程序
            services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler>();
        }
    }
}
