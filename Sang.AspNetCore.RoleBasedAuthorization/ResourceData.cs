namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 获取当前应用已使用权限的本地化详情。
    /// </summary>
    public class ResourceData
    {
        private static IReadOnlyList<ResourceInfo> _resources = Array.Empty<ResourceInfo>();

        /// <summary>
        /// 获取当前应用已使用权限的层级详情。
        /// </summary>
        public static IReadOnlyList<ResourceInfo> GetResourceInfos() => _resources;

        /// <summary>
        /// 设置当前应用控制器实际使用的权限。
        /// </summary>
        internal static void SetPermissions(IEnumerable<ResourceAttribute> permissions)
        {
            _resources = permissions
                .GroupBy(permission => permission.Resource!, StringComparer.OrdinalIgnoreCase)
                .Select(group => new ResourceInfo
                {
                    ResourceKey = group.Key,
                    ResourceName = group.First().ResourceName ?? group.Key,
                    Actions = group.Select(permission => new ResourceActionInfo
                    {
                        ActionKey = permission.Action,
                        ActionName = permission.ActionName,
                        Description = permission.Description,
                        Permission = permission.Permission
                    }).ToList()
                })
                .ToList();
        }
    }

    /// <summary>
    /// 权限管理界面使用的资源详情。
    /// </summary>
    public class ResourceInfo
    {
        /// <summary>
        /// 模块键。
        /// </summary>
        public string ResourceKey { get; init; } = string.Empty;

        /// <summary>
        /// 模块展示名称。
        /// </summary>
        public string ResourceName { get; init; } = string.Empty;

        /// <summary>
        /// 模块下实际使用的操作。
        /// </summary>
        public IReadOnlyList<ResourceActionInfo> Actions { get; init; } = Array.Empty<ResourceActionInfo>();
    }

    /// <summary>
    /// 权限模块下的操作详情。
    /// </summary>
    public class ResourceActionInfo
    {
        /// <summary>
        /// 操作键。
        /// </summary>
        public string ActionKey { get; init; } = string.Empty;

        /// <summary>
        /// 操作展示名称。
        /// </summary>
        public string ActionName { get; init; } = string.Empty;

        /// <summary>
        /// 操作介绍。
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 稳定的权限码。
        /// </summary>
        public string Permission { get; init; } = string.Empty;
    }
}
