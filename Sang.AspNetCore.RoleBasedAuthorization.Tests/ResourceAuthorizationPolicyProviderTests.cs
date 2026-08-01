using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Moq;

namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceAuthorizationPolicyProviderTests
    {
        private ResourceAuthorizationPolicyProvider CreateProvider()
        {
            var options = new AuthorizationOptions();
            var mockOptions = new Mock<IOptions<AuthorizationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(options);
            return new ResourceAuthorizationPolicyProvider(mockOptions.Object);
        }

        [Fact]
        public async Task GetPolicyAsync_NewPolicyName_CreatesPolicy()
        {
            var provider = CreateProvider();

            var policy = await provider.GetPolicyAsync("Resource-Action");

            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
            Assert.IsType<ResourceAttribute>(policy.Requirements[0]);
        }

        [Fact]
        public async Task GetPolicyAsync_CalledTwice_ReturnsSamePolicy()
        {
            var provider = CreateProvider();

            var policy1 = await provider.GetPolicyAsync("Resource-Action");
            var policy2 = await provider.GetPolicyAsync("Resource-Action");

            Assert.Same(policy1, policy2);
        }

        [Fact]
        public async Task GetPolicyAsync_ExistingPolicy_ReturnsExisting()
        {
            var options = new AuthorizationOptions();
            options.AddPolicy("ExistingPolicy", policy => policy.Requirements.Add(new ResourceAttribute("ExistingPolicy")));
            var mockOptions = new Mock<IOptions<AuthorizationOptions>>();
            mockOptions.Setup(x => x.Value).Returns(options);
            var provider = new ResourceAuthorizationPolicyProvider(mockOptions.Object);

            var policy = await provider.GetPolicyAsync("ExistingPolicy");

            Assert.NotNull(policy);
            Assert.Single(policy.Requirements);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceAuthorizationPolicyProvider(null!));
        }
    }
}
