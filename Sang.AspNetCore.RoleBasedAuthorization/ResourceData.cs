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
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="name">名称</param>
        public static void AddResource(string name)
        {
            AddResource(name, "");
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="action">操作</param>
        public static void AddResource(string name, string? action)
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
        }

        /// <summary>
        /// 资源信息
        /// </summary>
        public static Dictionary<string, List<string>> Resources { get; set; }
    }
}
