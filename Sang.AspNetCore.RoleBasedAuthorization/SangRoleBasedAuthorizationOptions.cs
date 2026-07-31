namespace Sang.AspNetCore.RoleBasedAuthorization
{
    /// <summary>
    /// Sang.AspNetCore.RoleBasedAuthorization 配置项
    /// </summary>
    public class SangRoleBasedAuthorizationOptions
    {
        /// <summary>
        /// 超级管理员角色名，拥有该角色的用户将跳过权限检查。
        /// 默认为内置的 <see cref="ResourceRole.Administrator"/>。
        /// </summary>
        public string AdministratorRoleName { get; set; } = ResourceRole.Administrator;
    }
}
