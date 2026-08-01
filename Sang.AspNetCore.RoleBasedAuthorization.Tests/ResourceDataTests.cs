namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceDataTests
    {
        [Fact]
        public void GetResourceInfos_ReturnsHierarchicalPermissionDetails()
        {
            ResourceData.SetPermissions(new[]
            {
                new ResourceAttribute("roles", "read", "角色权限", "查看角色列表", "允许浏览系统角色定义"),
                new ResourceAttribute("roles", "delete", "角色权限", "删除角色", "允许删除非系统内置角色")
            });

            var resourceInfo = Assert.Single(ResourceData.GetResourceInfos());
            Assert.Equal("roles", resourceInfo.ResourceKey);
            Assert.Equal("角色权限", resourceInfo.ResourceName);
            Assert.Equal(2, resourceInfo.Actions.Count);
            Assert.Contains(resourceInfo.Actions, action => action.Permission == "roles.read" && action.ActionName == "查看角色列表");
            Assert.Contains(resourceInfo.Actions, action => action.Permission == "roles.delete" && action.Description == "允许删除非系统内置角色");
        }
    }
}