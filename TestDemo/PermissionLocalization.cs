using Sang.AspNetCore.RoleBasedAuthorization;

namespace TestDemo
{
    /// <summary>
    /// 基于配置的权限文案本地化选项。
    /// </summary>
    public sealed class PermissionLocalizationOptions
    {
        /// <summary>
        /// 配置节名称。
        /// </summary>
        public const string SectionName = "PermissionLocalization";

        /// <summary>
        /// 未指定或不支持语言时使用的默认语言。
        /// </summary>
        public string DefaultCulture { get; set; } = "zh-CN";

        /// <summary>
        /// 按语言代码组织的权限文案。
        /// </summary>
        public Dictionary<string, Dictionary<string, PermissionText>> Cultures { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 权限项的展示文案。
    /// </summary>
    public sealed class PermissionText
    {
        /// <summary>
        /// 展示名称。
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 展示介绍。
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// 按权限键解析展示文案。
    /// </summary>
    public interface IPermissionLocalizer
    {
        /// <summary>
        /// 解析受支持的语言代码。
        /// </summary>
        string ResolveCulture(string? requestedCulture);

        /// <summary>
        /// 获取权限文案，找不到时使用默认文案。
        /// </summary>
        PermissionText Localize(string culture, string key, string fallbackName, string? fallbackDescription = null);
    }

    /// <summary>
    /// 从应用配置读取权限文案。
    /// </summary>
    public sealed class ConfigurationPermissionLocalizer : IPermissionLocalizer
    {
        private readonly PermissionLocalizationOptions _options;

        /// <summary>
        /// 初始化配置本地化服务。
        /// </summary>
        public ConfigurationPermissionLocalizer(PermissionLocalizationOptions options)
        {
            _options = options;
        }

        /// <inheritdoc />
        public string ResolveCulture(string? requestedCulture)
        {
            if (!string.IsNullOrWhiteSpace(requestedCulture) && FindCulture(requestedCulture) is not null)
            {
                return requestedCulture;
            }

            return _options.DefaultCulture;
        }

        /// <inheritdoc />
        public PermissionText Localize(string culture, string key, string fallbackName, string? fallbackDescription = null)
        {
            var text = FindCulture(culture)?.GetValueOrDefault(key);
            if (text is null && !string.Equals(culture, _options.DefaultCulture, StringComparison.OrdinalIgnoreCase))
            {
                text = FindCulture(_options.DefaultCulture)?.GetValueOrDefault(key);
            }

            return new PermissionText
            {
                Name = text?.Name ?? fallbackName,
                Description = text?.Description ?? fallbackDescription
            };
        }

        private Dictionary<string, PermissionText>? FindCulture(string culture)
        {
            if (_options.Cultures.TryGetValue(culture, out var texts))
            {
                return texts;
            }

            var language = culture.Split('-', 2)[0];
            return _options.Cultures.FirstOrDefault(item =>
                string.Equals(item.Key.Split('-', 2)[0], language, StringComparison.OrdinalIgnoreCase)).Value;
        }
    }

    /// <summary>
    /// 本地化后的权限模块详情。
    /// </summary>
    public sealed class LocalizedResourceInfo
    {
        /// <summary>
        /// 模块键。
        /// </summary>
        public string ResourceKey { get; init; } = string.Empty;

        /// <summary>
        /// 本地化模块名称。
        /// </summary>
        public string ResourceName { get; init; } = string.Empty;

        /// <summary>
        /// 本地化操作列表。
        /// </summary>
        public IReadOnlyList<LocalizedResourceActionInfo> Actions { get; init; } = Array.Empty<LocalizedResourceActionInfo>();
    }

    /// <summary>
    /// 本地化后的权限操作详情。
    /// </summary>
    public sealed class LocalizedResourceActionInfo
    {
        /// <summary>
        /// 操作键。
        /// </summary>
        public string ActionKey { get; init; } = string.Empty;

        /// <summary>
        /// 本地化操作名称。
        /// </summary>
        public string ActionName { get; init; } = string.Empty;

        /// <summary>
        /// 本地化操作介绍。
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// 稳定权限码。
        /// </summary>
        public string Permission { get; init; } = string.Empty;
    }

    /// <summary>
    /// 将资源详情映射为本地化展示数据。
    /// </summary>
    public static class PermissionLocalizationExtensions
    {
        /// <summary>
        /// 使用指定语言本地化资源详情。
        /// </summary>
        public static IReadOnlyList<LocalizedResourceInfo> Localize(
            this IEnumerable<ResourceInfo> resources,
            IPermissionLocalizer localizer,
            string culture)
        {
            return resources.Select(resource =>
            {
                var resourceText = localizer.Localize(culture, resource.ResourceKey, resource.ResourceName);
                return new LocalizedResourceInfo
                {
                    ResourceKey = resource.ResourceKey,
                    ResourceName = resourceText.Name!,
                    Actions = resource.Actions.Select(action =>
                    {
                        var actionText = localizer.Localize(culture, action.Permission, action.ActionName, action.Description);
                        return new LocalizedResourceActionInfo
                        {
                            ActionKey = action.ActionKey,
                            ActionName = actionText.Name!,
                            Description = actionText.Description,
                            Permission = action.Permission
                        };
                    }).ToList()
                };
            }).ToList();
        }
    }
}