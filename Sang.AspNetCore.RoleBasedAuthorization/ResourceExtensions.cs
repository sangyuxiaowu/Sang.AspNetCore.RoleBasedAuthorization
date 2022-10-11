using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    public static class ResourceExtensions
    {
        /// <summary>
        /// 添加 Sang.AspNetCore.RoleBasedAuthorization 服务
        /// </summary>
        /// <param name="Services"></param>
        public static void AddSangRoleBasedAuthorization(this IServiceCollection Services)
        {
            // 注册定义的 IApplicationModelProvider 用于获取到全局定义的 Resource 特性的信息
            Services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, ResourceInfoModelProvider>());
            // 添加自定义授权策略
            Services.AddSingleton<IAuthorizationPolicyProvider, ResourceAuthorizationPolicyProvider>();
            // 添加自定义授权处理程序
            Services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler>();
        }
    }
}
