using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 模型和提供程序
    /// 在服务启动时从配置的Resource特性中获取权限类别信息
    /// https://docs.microsoft.com/zh-cn/aspnet/core/mvc/controllers/application-model?view=aspnetcore-6.0
    /// </summary>
    internal sealed class ResourceInfoModelProvider : IApplicationModelProvider
    {
        /// <summary>
        /// 执行的排序
        /// 首先 (Order=-1000)：
        /// DefaultApplicationModelProvider
        /// 然后是这个，用于补全 ResourceAttribute 的模块信息：
        /// AuthorizationApplicationModelProvider
        /// CorsApplicationModelProvider
        /// 接着是 AuthorizationApplicationModelProvider
        /// </summary>
        public int Order => -991;

        /// <summary>
        /// 基于其 Order 属性以倒序调用
        /// </summary>
        /// <param name="context"></param>
        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            //获取所有的控制器
            List<ResourceAttribute> attributeData = new List<ResourceAttribute>();
            foreach (var controllerModel in context.Result.Controllers)
            {
                //得到ResourceAttribute

                //Controller 的特性
                var resourceData = controllerModel.Attributes.OfType<ResourceAttribute>().ToArray();
                if (resourceData.Length > 0)
                {
                    attributeData.AddRange(resourceData);
                }
                //Controller 中的每个方法的特性
                foreach (var actionModel in controllerModel.Actions)
                {
                    var actionResourceData = actionModel.Attributes.OfType<ResourceAttribute>().ToArray();
                    if (actionResourceData.Length > 0)
                    {
                        attributeData.AddRange(actionResourceData);
                    }
                }
            }
            ResourceData.SetPermissions(attributeData.DistinctBy(attribute => attribute.Permission, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 基于其 Order 属性以升序调用
        /// </summary>
        /// <param name="context"></param>
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            foreach (var controller in context.Result.Controllers)
            {
                var module = controller.Attributes.OfType<ResourceModuleAttribute>().SingleOrDefault();
                foreach (var action in controller.Actions)
                {
                    foreach (var resource in action.Attributes.OfType<ResourceAttribute>())
                    {
                        if (resource.Resource is null)
                        {
                            if (module is null)
                            {
                                throw new InvalidOperationException($"操作 '{resource.Action}' 未定义模块，请在 Controller 上添加 ResourceModuleAttribute 或使用完整的 ResourceAttribute 构造函数。");
                            }

                            resource.SetModule(module);
                        }
                    }
                }
            }
        }
    }

}