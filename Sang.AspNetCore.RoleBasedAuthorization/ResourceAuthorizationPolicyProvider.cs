using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 资源授权策略
    /// 实现动态 AddPolicy
    /// </summary>
    internal sealed class ResourceAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private AuthorizationOptions _options;
        /// <summary>
        /// 构造资源授权策略
        /// </summary>
        /// <param name="options">配置信息</param>
        /// <exception cref="ArgumentNullException">授权配置为空</exception>
        public ResourceAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options.Value;
        }

        /// <summary>
        /// 默认策略
        /// 在未指定策略名称的情况下为 [Authorize] 属性提供授权策略
        /// </summary>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(_options.DefaultPolicy);
        }

        /// <summary>
        /// 回退策略
        /// 以提供在合并策略和未指定策略时使用的策略
        /// </summary>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
            Task.FromResult<AuthorizationPolicy>(null);


        /// <summary>
        /// 自定义授权策略
        /// 自动增加 Policy 授权策略
        /// </summary>
        /// <param name="policyName">授权名称</param>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // 检查这个授权策略有没有
            AuthorizationPolicy? policy = _options.GetPolicy(policyName);

            if (policy is null)
            {
                _options.AddPolicy(policyName, builder =>
                {
                    builder.AddRequirements(new ResourceAttribute(policyName));
                });
            }

            return Task.FromResult(_options.GetPolicy(policyName));
        }
    }

}