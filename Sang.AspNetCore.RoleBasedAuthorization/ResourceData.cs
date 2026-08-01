using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 全局存储或获取项目中含有的 Resource 特性
    /// </summary>
    public class ResourceData
    {
        static ResourceData()
        {
            Resources = new Dictionary<string, List<string>>();
            ResourceInfos = new List<ResourceInfo>();
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="name">名称</param>
        public static void AddResource(string name)
        {
            AddResource(name, null, null);
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="action">操作</param>
        /// <param name="description">介绍</param>
        public static void AddResource(string name, string? action, string? description = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (!Resources.ContainsKey(name))
            {
                Resources.Add(name, new List<string>());
            }

            if (!string.IsNullOrEmpty(action) && !Resources[name].Contains(action))
            {
                Resources[name].Add(action);
            }

            var resourceInfo = ResourceInfos.FirstOrDefault(item =>
                string.Equals(item.Resource, name, StringComparison.Ordinal)
                && string.Equals(item.Action, action, StringComparison.Ordinal));
            if (resourceInfo is null)
            {
                ResourceInfos.Add(new ResourceInfo
                {
                    Resource = name,
                    Action = action,
                    Description = description
                });
            }
            else if (!string.IsNullOrEmpty(description)
                && !string.IsNullOrEmpty(resourceInfo.Description)
                && !string.Equals(resourceInfo.Description, description, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"资源 '{name}' 的操作 '{action}' 定义了不同的介绍。");
            }
            else if (!string.IsNullOrEmpty(description))
            {
                resourceInfo.Description = description;
            }
        }

        /// <summary>
        /// 资源信息
        /// </summary>
        public static Dictionary<string, List<string>> Resources { get; set; }

        /// <summary>
        /// 包含模块、操作及介绍的资源详情。
        /// </summary>
        public static List<ResourceInfo> ResourceInfos { get; set; }
    }

    /// <summary>
    /// 权限管理界面使用的资源详情。
    /// </summary>
    public class ResourceInfo
    {
        /// <summary>
        /// 模块名称。
        /// </summary>
        public string Resource { get; set; } = string.Empty;

        /// <summary>
        /// 操作名称。
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// 操作介绍。
        /// </summary>
        public string? Description { get; set; }
    }
}
