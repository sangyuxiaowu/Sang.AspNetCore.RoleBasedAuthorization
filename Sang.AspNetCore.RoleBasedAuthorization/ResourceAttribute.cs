using Microsoft.AspNetCore.Authorization;

namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// 资源描述属性
    /// 描述访问的角色需要的资源要求
    /// 填写单独的整个资源，或使用 Action 设置资源下的某个操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResourceAttribute: AuthorizeAttribute
    {
        private string _resouceName;
        private string? _action;
        /// <summary>
        /// 设置资源类型
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <exception cref="ArgumentNullException">资源名称不能为空</exception>
        public ResourceAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            _resouceName = name;
            //把资源名称设置成Policy名称
            Policy = _resouceName;
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        /// <returns></returns>
        public string GetResource()
        {
            return _resouceName;
        }

        /// <summary>
        /// 获取操作名称
        /// </summary>
        public string? Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
                if (!string.IsNullOrEmpty(value))
                {
                    //把资源名称跟操作名称组装成Policy
                    Policy = _resouceName + "-" + value;
                }
            }
        }
    }
}