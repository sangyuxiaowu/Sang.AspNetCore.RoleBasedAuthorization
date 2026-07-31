using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceAuthorizationHandlerTests
    {
        private readonly SangRoleBasedAuthorizationOptions _defaultOptions = new();

        private ResourceAuthorizationHandler CreateHandler(string? administratorRoleName = null)
        {
            var options = new SangRoleBasedAuthorizationOptions
            {
                AdministratorRoleName = administratorRoleName ?? _defaultOptions.AdministratorRoleName
            };
            var mockOptions = new Mock<IOptions<SangRoleBasedAuthorizationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(options);
            return new ResourceAuthorizationHandler(mockOptions.Object);
        }

        private AuthorizationHandlerContext CreateContext(ClaimsPrincipal user, ResourceAttribute requirement)
        {
            return new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null);
        }

        [Fact]
        public async Task HandleRequirementAsync_AdministratorRole_Succeeds()
        {
            var handler = CreateHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, ResourceRole.Administrator)
            }));
            var requirement = new ResourceAttribute("Resource", "Action");
            var context = CreateContext(user, requirement);

            await handler.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_CustomAdministratorRole_Succeeds()
        {
            var handler = CreateHandler("Admin");
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }));
            var requirement = new ResourceAttribute("Resource", "Action");
            var context = CreateContext(user, requirement);

            await handler.HandleAsync(context);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NoMatchingClaims_DoesNotSucceed()
        {
            var handler = CreateHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "User")
            }));
            var requirement = new ResourceAttribute("Resource", "Action");
            var context = CreateContext(user, requirement);

            await handler.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }

        [Theory]
        [InlineData("Resource", "Resource", true)]
        [InlineData("Resource", "Other", false)]
        [InlineData("Resource-Action", "Resource", true)]
        [InlineData("Resource-Action", "Resource-Action", true)]
        [InlineData("Resource-Action", "Resource-Other", false)]
        [InlineData("Resource-Action", "Resource-*", true)]
        [InlineData("Resource-Action", "*", true)]
        public async Task HandleRequirementAsync_PermissionClaim_MatchesCorrectly(string requirementName, string permissionValue, bool expectedSuccess)
        {
            var handler = CreateHandler();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ResourceClaimTypes.Permission, permissionValue)
            }));
            var requirement = new ResourceAttribute(requirementName);
            var context = CreateContext(user, requirement);

            await handler.HandleAsync(context);

            Assert.Equal(expectedSuccess, context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_NullUser_DoesNotSucceed()
        {
            var handler = CreateHandler();
            var requirement = new ResourceAttribute("Resource", "Action");
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                null!,
                null);

            await handler.HandleAsync(context);

            Assert.False(context.HasSucceeded);
        }
    }
}
