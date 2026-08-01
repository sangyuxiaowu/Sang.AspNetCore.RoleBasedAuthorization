using Microsoft.AspNetCore.Http;
using Moq;
using Sang.AspNetCore.RoleBasedAuthorization.RolePermission;
using System.Security.Claims;

namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class RolePermissionMiddlewareTests
    {
        [Fact]
        public async Task Invoke_MergesRoleAndUserPermissions_AndRemovesDuplicates()
        {
            var rolePermission = new Mock<IRolePermission>();
            rolePermission
                .Setup(service => service.GetRolePermissionClaimsByName("Reader"))
                .ReturnsAsync(new List<Claim>
                {
                    new(ResourceClaimTypes.Permission, "Books-Read")
                });
            rolePermission
                .Setup(service => service.GetRolePermissionClaimsByName("Editor"))
                .ReturnsAsync(new List<Claim>
                {
                    new(ResourceClaimTypes.Permission, "Books-Read"),
                    new(ResourceClaimTypes.Permission, "Books-Write")
                });
            rolePermission
                .Setup(service => service.GetUserPermissionClaims(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new List<Claim>
                {
                    new(ResourceClaimTypes.Permission, "Books-Write"),
                    new(ResourceClaimTypes.Permission, "Books-Delete")
                });
            var middleware = new RolePermissionMiddleware(
                _ => Task.CompletedTask,
                new RolePermissionOptions { Always = true },
                rolePermission.Object);
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Reader"),
                new Claim(ClaimTypes.Role, "Editor")
            }));

            await middleware.Invoke(context);

            var permissions = context.User.FindAll(ResourceClaimTypes.Permission).ToList();
            Assert.Equal(3, permissions.Count);
            Assert.Contains(permissions, claim => claim.Value == "Books-Read");
            Assert.Contains(permissions, claim => claim.Value == "Books-Write");
            Assert.Contains(permissions, claim => claim.Value == "Books-Delete");
            rolePermission.Verify(service => service.GetUserPermissionClaims(context.User), Times.Once);
        }
    }
}