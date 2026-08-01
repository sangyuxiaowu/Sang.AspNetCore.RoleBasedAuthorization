using Microsoft.AspNetCore.Authorization;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 声明资源操作的授权要求及默认展示文本。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResourceAttribute: AuthorizeAttribute, IAuthorizationRequirement
    {
        /// <summary>
        /// 在 <see cref="ResourceModuleAttribute"/> 所在 Controller 中初始化资源操作授权要求。
        /// </summary>
        /// <param name="action">稳定的英文操作键。</param>
        /// <param name="actionName">操作展示名称。</param>
        /// <param name="description">操作介绍。</param>
        public ResourceAttribute(string action, string actionName, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("操作键不能为空。", nameof(action));
            }

            Action = action;
            ActionName = actionName;
            Description = description;
            Policy = action;
        }

        /// <summary>
        /// 初始化包含完整模块信息的资源操作授权要求。
        /// </summary>
        /// <param name="resource">稳定的英文模块键。</param>
        /// <param name="action">稳定的英文操作键。</param>
        /// <param name="resourceName">模块展示名称。</param>
        /// <param name="actionName">操作展示名称。</param>
        /// <param name="description">操作介绍。</param>
        public ResourceAttribute(string resource, string action, string resourceName, string actionName, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException("资源键不能为空。", nameof(resource));
            }
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException("操作键不能为空。", nameof(action));
            }

            Resource = resource;
            Action = action;
            ResourceName = resourceName;
            ActionName = actionName;
            Description = description;
            Policy = Permission;
        }

        internal ResourceAttribute(string resource, string action)
        {
            Resource = resource;
            Action = action;
            ActionName = action;
            Policy = Permission;
        }

        /// <summary>
        /// 稳定的资源键。
        /// </summary>
        public string? Resource { get; private set; }

        /// <summary>
        /// 稳定的操作键。
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// 模块展示名称。
        /// </summary>
        public string? ResourceName { get; private set; }

        /// <summary>
        /// 操作展示名称。
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// 操作介绍。
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// 权限 Claim 与授权策略使用的稳定权限码。
        /// </summary>
        public string Permission => $"{Resource}.{Action}";

        internal void SetModule(ResourceModuleAttribute module)
        {
            if (Resource is not null)
            {
                return;
            }

            Resource = module.Resource;
            ResourceName = module.Name;
            Policy = Permission;
        }
    }

    /// <summary>
    /// 为 Controller 中的资源操作提供默认模块信息。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ResourceModuleAttribute : Attribute
    {
        /// <summary>
        /// 初始化模块定义。
        /// </summary>
        /// <param name="resource">稳定的英文模块键。</param>
        /// <param name="name">模块展示名称。</param>
        public ResourceModuleAttribute(string resource, string name)
        {
            Resource = string.IsNullOrWhiteSpace(resource) ? throw new ArgumentException("模块键不能为空。", nameof(resource)) : resource;
            Name = name;
        }

        /// <summary>
        /// 稳定的模块键。
        /// </summary>
        public string Resource { get; }

        /// <summary>
        /// 模块展示名称。
        /// </summary>
        public string Name { get; }
    }
}