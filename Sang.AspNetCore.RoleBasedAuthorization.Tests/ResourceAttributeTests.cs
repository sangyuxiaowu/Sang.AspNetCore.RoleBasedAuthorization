using Microsoft.AspNetCore.Authorization;

namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceAttributeTests
    {
        [Theory]
        [InlineData("roles", "read")]
        [InlineData("values", "delete")]
        public void Constructor_WithCompleteMetadata_SetsPermissionPolicy(string resource, string action)
        {
            var attr = new ResourceAttribute(resource, action, "Roles", "Read roles", "Allows viewing roles");

            Assert.Equal(resource, attr.Resource);
            Assert.Equal("Roles", attr.ResourceName);
            Assert.Equal("Read roles", attr.ActionName);
            Assert.Equal($"{resource}.{action}", attr.Permission);
            Assert.Equal(attr.Permission, attr.Policy);
        }

        [Fact]
        public void Constructor_WithEmptyKeys_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ResourceAttribute("", "read", "Roles", "Read roles"));
            Assert.Throws<ArgumentException>(() => new ResourceAttribute("roles", "", "Roles", "Read roles"));
        }

        [Fact]
        public void Implements_AuthorizeAttribute_And_IAuthorizationRequirement()
        {
            var attr = new ResourceAttribute("read", "Read roles");

            Assert.IsAssignableFrom<AuthorizeAttribute>(attr);
            Assert.IsAssignableFrom<IAuthorizationRequirement>(attr);
        }

        [Fact]
        public void SetModule_CombinesModuleAndAction()
        {
            var attr = new ResourceAttribute("delete", "Delete role", "Allows deleting a role");
            attr.SetModule(new ResourceModuleAttribute("roles", "Role permissions"));

            Assert.Equal("roles.delete", attr.Permission);
            Assert.Equal("Role permissions", attr.ResourceName);
            Assert.Equal(attr.Permission, attr.Policy);
        }

    }
}
