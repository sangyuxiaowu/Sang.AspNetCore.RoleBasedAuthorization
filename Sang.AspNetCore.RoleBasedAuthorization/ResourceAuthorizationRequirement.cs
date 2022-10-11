using Microsoft.AspNetCore.Authorization;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    internal class ResourceAuthorizationRequirement : IAuthorizationRequirement
    {

        /// <summary>
        /// 资源
        /// </summary>
        public string Resource { get; }
        /// <summary>
        /// 动作
        /// </summary>
        public string? Action { get; }

        /// <summary>
        /// 授权要求策略
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="action">动作</param>

        public ResourceAuthorizationRequirement(string resource, string? action)
        {
            Resource = resource;
            Action = action;
        }
    }
}