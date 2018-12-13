

using apim_utils.Models;
using apim_utils.Supports;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace apim_utils.Experiments
{
    public class ApimClient
    {
        private Action<HttpClient> _tokenLambda;

        private ApimClient() { }

        public static async Task<ApimClient> BuildAsync()
        {
            var apim = new ApimClient
            {
                _tokenLambda = await TokenHelper.GetAzureAuthToken()
            };
            return apim;
        }


        private static string GetBasePath()
        {
            return $"/subscriptions/{Constants.SubscriptionId}/resourceGroups/{Constants.ResourceGroupName}/providers/Microsoft.ApiManagement/service/{Constants.ServiceName}";
        }

        public async Task CreateOrUpdateAsync(string apiName, ApiSpecificationPayload specification)
        {
            var path = $"{GetBasePath()}/apis/{apiName}?api-version=2018-06-01-preview";

            await Constants.Azure
                .PutRestAsync(path, specification, (_tokenLambda).AddIfMatch());
        }

        public async Task AddApiToProductAsync(string product, string apiName)
        {
            var path = $"{GetBasePath()}/products/{product}/apis/{apiName}?api-version=2018-06-01-preview";
            var response = await Constants.Azure
                    .PutRestAsync(path, string.Empty, _tokenLambda);
        }

        public async Task AddRevisionAsync(
            string apiName, string revision,
            ApiSpecificationPayload specification)
        {
            var path = $"{GetBasePath()}/apis/{apiName};rev={revision}?api-version=2018-06-01-preview";
            var response = await Constants.Azure
                .PutRestAsync(path, specification, (_tokenLambda).AddIfMatch());
        }

        public async Task ReleaseRevisionAsync(string apiId, string revision, string remark)
        {
            var path = $"{GetBasePath()}/apis/{apiId}/releases/{Guid.NewGuid().ToString("N")}?api-version=2017-03-01";
            var lambda = (_tokenLambda).AddIfMatch();

            await Constants.Azure.PutRestAsync(path,
                new
                {
                    apiId = $"/apis/{apiId};rev={revision}",
                    notes = remark
                }, lambda);
        }

        public async Task CreateVersionFromRevisionAsync(
            string apiId, string revision, string versionName,
            string versionDescription, string newApiId,
            ApiVersionSchemes scheme)
        {
            var path = $"{GetBasePath()}/apis/{newApiId}?api-version=2017-03-01";
            var lambda = (_tokenLambda)
                .AddIfMatch()
                .AddSpecialContentType("application/vnd.ms-azure-apim.revisioninfo+json");
            await Constants.Azure.PutRestAsync(path,
                new
                {
                    SourceApiId = $"/apis/{apiId}[{revision}]",
                    ApiVersionName = versionName,
                    Name = apiId,
                    Path = newApiId,
                    Protocols = new string[] { "https" },
                    ApiVersionDescription = versionDescription,
                    ApiVersionSet = new
                    {
                        VersioningScheme = scheme.ToString()
                    }
                }, lambda);
        }

        public async Task CreateOrUpdateApiAsync(string apiId, object payload)
        {
            var path = $"{GetBasePath()}/apis/{apiId}?api-version=2017-03-01";
            await Constants.Azure
                .PutRestAsync(path, payload, (_tokenLambda).AddIfMatch());
        }

        public async Task CreateOrUpdateVersionSetAsync(
            string setName, string displayName, string description, ApiVersionSchemes scheme)
        {
            var path = $"{GetBasePath()}/api-version-sets/{setName}?api-version=2018-06-01-preview";
            await Constants.Azure
                .PutRestAsync(path, new
                {
                    Properties = new
                    {
                        DisplayName = displayName,
                        Description = description,
                        VersioningScheme = scheme.ToString()
                    }
                }, (_tokenLambda).AddIfMatch());
        }

        public async Task<VersionSchemeCollection> ListApiVersionSchemeAsync()
        {
            var path = $"{GetBasePath()}/api-version-sets?api-version=2018-06-01-preview";

            return await Constants.Azure
                .GetRestAsync<VersionSchemeCollection>(path, this._tokenLambda);
        }

        public async Task DeleteAsync(string apiId)
        {
            var path = $"{GetBasePath()}/apis/{apiId}?api-version=2018-06-01-preview";
            await Constants.Azure.DeleteRestAsync(path, (_tokenLambda).AddIfMatch());
        }

        public async Task<ApiSpecificationCollection> ListAsync()
        {
            var path = "/apis?api-version=2018-06-01-preview";
            var apiSpecs = await Constants.APIMUri
                .GetRestAsync<ApiSpecificationCollection>(path,
                TokenHelper.GetAuthSasHeader(Constants.Sas));
            return apiSpecs;
        }
    }
}
