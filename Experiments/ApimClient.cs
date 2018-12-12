using apim_utils.Supports;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace apim_utils.Experiments
{
    public class ApimClient
    {
        private static string GetBasePath()
        {
            return $"/subscriptions/{Constants.SubscriptionId}/resourceGroups/{Constants.ResourceGroupName}/providers/Microsoft.ApiManagement/service/{Constants.ServiceName}";
        }

        public async Task ReleaseRevisionAsync(string apiId, string revision)
        {
            var path = $"{GetBasePath()}/apis/{apiId}/releases/{Guid.NewGuid().ToString("N")}?api-version=2017-03-01";
            var lambda = (await TokenHelper.GetAzureAuthToken())
                .AddIfMatch();

            await Constants.Azure.PutRestAsync(path,
                new
                {
                    apiId = $"/apis/{apiId};rev={revision}",
                    notes = "Let's release it"
                }, lambda);
        }

        public async Task CreateVersionFromRevisionAsync(string apiId, string revision, string newApiId)
        {
            var path = $"{GetBasePath()}/apis/{newApiId}?api-version=2017-03-01";
            var lambda = (await TokenHelper.GetAzureAuthToken())
                .AddIfMatch()
                .AddSpecialContentType("application/vnd.ms-azure-apim.revisioninfo+json");
            await Constants.Azure.PutRestAsync(path,
                new
                {
                    SourceApiId = $"/apis/{apiId}[{revision}]",
                    ApiVersionName = "v2",
                    Name = apiId,
                    Path = newApiId,
                    Protocols = new string[] { "https" },
                    ApiVersionDescription = "Description",
                    ApiVersionSet = new
                    {
                        VersioningScheme = "Segment"
                    }
                }, lambda);
        }

        public async Task CreateOrUpdateApiAsync(string apiId, object payload)
        {
            var path = $"{GetBasePath()}/apis/{apiId}?api-version=2017-03-01";
            await Constants.Azure
                .PutRestAsync(path, payload, (await TokenHelper.GetAzureAuthToken()).AddIfMatch());
        }

        public async Task CreateOrUpdateVersionSetAsync(
            string setName, string displayName, string description)
        {
            var path = $"{GetBasePath()}/api-version-sets/{setName}?api-version=2018-06-01-preview";
            await Constants.Azure
                .PutRestAsync(path, new
                {
                    Properties = new
                    {
                        DisplayName = displayName,
                        Description = description,
                        VersioningScheme = "Segment"
                    }
                }, (await TokenHelper.GetAzureAuthToken()).AddIfMatch());
        }
    }
}
