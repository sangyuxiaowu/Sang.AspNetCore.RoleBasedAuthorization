namespace Sang.AspNetCore.RoleBasedAuthorization.Tests
{
    public class ResourceDataTests
    {
        [Fact]
        public void AddResource_WithDescription_AddsResourceInfo()
        {
            var originalResources = ResourceData.Resources;
            var originalResourceInfos = ResourceData.ResourceInfos;
            ResourceData.Resources = new Dictionary<string, List<string>>();
            ResourceData.ResourceInfos = new List<ResourceInfo>();

            try
            {
                ResourceData.AddResource("Roles", "View", "Allows viewing system role definitions");

                var resourceInfo = Assert.Single(ResourceData.ResourceInfos);
                Assert.Equal("Roles", resourceInfo.Resource);
                Assert.Equal("View", resourceInfo.Action);
                Assert.Equal("Allows viewing system role definitions", resourceInfo.Description);
                Assert.Equal(new[] { "View" }, ResourceData.Resources["Roles"]);
            }
            finally
            {
                ResourceData.Resources = originalResources;
                ResourceData.ResourceInfos = originalResourceInfos;
            }
        }
    }
}