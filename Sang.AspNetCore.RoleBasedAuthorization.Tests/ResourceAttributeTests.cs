using Microsoft.AspNetCore.Authorization;

namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceAttributeTests
    {
        [Theory]
        [InlineData("资源")]
        [InlineData("Resource")]
        public void Constructor_WithResourceOnly_SetsPolicyToResource(string resource)
        {
            var attr = new ResourceAttribute(resource);

            Assert.Equal(resource, attr.GetResource());
            Assert.Null(attr.Action);
            Assert.Equal(resource, attr.Policy);
        }

        [Theory]
        [InlineData("资源", "操作")]
        [InlineData("Resource", "Action")]
        public void Constructor_WithResourceAndAction_SetsPolicy(string resource, string action)
        {
            var attr = new ResourceAttribute(resource, action);

            Assert.Equal(resource, attr.GetResource());
            Assert.Equal(action, attr.Action);
            Assert.Equal($"{resource}-{action}", attr.Policy);
        }

        [Theory]
        [InlineData("资源-操作", "资源", "操作")]
        [InlineData("Resource-Action", "Resource", "Action")]
        public void Constructor_WithDashNotation_SplitsResourceAndAction(string name, string expectedResource, string expectedAction)
        {
            var attr = new ResourceAttribute(name);

            Assert.Equal(expectedResource, attr.GetResource());
            Assert.Equal(expectedAction, attr.Action);
            Assert.Equal(name, attr.Policy);
        }

        [Fact]
        public void Constructor_WithDashNotation_MultipleDashes_TakesFirstPartAsResource()
        {
            var attr = new ResourceAttribute("资源-操作-额外");

            Assert.Equal("资源", attr.GetResource());
            Assert.Equal("操作", attr.Action);
            Assert.Equal("资源-操作", attr.Policy);
        }

        [Fact]
        public void Constructor_WithNullResource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceAttribute(null!));
        }

        [Fact]
        public void Constructor_WithEmptyResource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResourceAttribute(""));
        }

        [Fact]
        public void Implements_AuthorizeAttribute_And_IAuthorizationRequirement()
        {
            var attr = new ResourceAttribute("Resource", "Action");

            Assert.IsAssignableFrom<AuthorizeAttribute>(attr);
            Assert.IsAssignableFrom<IAuthorizationRequirement>(attr);
        }
    }
}
