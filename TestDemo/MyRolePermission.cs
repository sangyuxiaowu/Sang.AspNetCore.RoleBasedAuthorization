using Sang.AspNetCore.RoleBasedAuthorization;
using Sang.AspNetCore.RoleBasedAuthorization.RolePermission;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Claims;
using System.Xml.Linq;

namespace TestDemo
{

    /// <summary>
    /// 使用演示
    /// 请根据业务实现 GetRolePermissionClaimsByName 即可
    /// </summary>
    public class MyRolePermission : IRolePermission
    {

        public Task<List<Claim>> GetRolePermissionClaimsByName(string roleName)
        {
            List<Claim> list = new();
            if (MyRole.ContainsKey(roleName))
            {
                foreach (var item in MyRole[roleName])
                {
                    list.Add(new Claim(ResourceClaimTypes.Permission, item));
                }
            }
            return Task.FromResult(list);
        }


        // 以下为演示，实际使用可结合 Identity RoleManager 实现，并添加缓存
        public static Dictionary<string, List<string>> MyRole { get; set; } = new();

        /// <summary>
        /// 添加角色权限
        /// </summary>
        /// <param name="name">角色</param>
        /// <param name="action">权限</param>
        public static void AddRole(string name, string per)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(per))
            {
                return;
            }
            if (!MyRole.ContainsKey(name))
            {
                MyRole.Add(name, new List<string>());
            }

            if (!MyRole[name].Contains(per))
            {
                MyRole[name].Add(per);
            }
        }
    }
}
