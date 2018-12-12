

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using apim_utils.Models;
using System.Linq;
using apim_utils.Supports;

namespace apim_utils
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAppAsync().Wait();
        }

        private async static Task RunAppAsync()
        {
            var apiName = "petstore";
            var product = "Unlimited";

            // await DeleteAsync(apiName);

            var petStoreApiSpec = new ApiSpecificationPayload
            {
                Properties = new ApSpecCreatePayloadProperties
                {
                    ContentFormat = "swagger-link-json",
                    ContentValue = "http://petstore.swagger.io/v2/swagger.json",
                    Path = "petstore/v1"
                }
            };

            await CreateOrUpdateAsync(petStoreApiSpec);
            await AddApiToProductAsync(product, apiName);

            var betaSpec = new ApiSpecificationPayload
            {
                Properties = new ApSpecRevisionPayloadProperties
                {
                    IsOnline = true,
                    DisplayName = "Swagger Petstore",
                    Description = "Changed description",
                    ServiceUrl = "http://petstore.swagger.io/v5",
                    ApiRevisionDescription = "Moved the backend too",
                    Path = "petstore/beta",
                    Protocols = new string[] { "https" }
                }
            };
            await AddRevisionAsync(apiName, "3", betaSpec);
        }

        public async static Task CreateOrUpdateVersionSetAsync(
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

        public async static Task<VersionSchemeCollection> ListApiVersionSchemeAsync()
        {
            var path = $"{GetBasePath()}/api-version-sets?api-version=2018-06-01-preview";

            var response = await Constants.Azure
                .GetRestAsync<VersionSchemeCollection>(path, await TokenHelper.GetAzureAuthToken());

            return response;
        }

        private async static Task AddRevisionAsync(
            string apiName, string revision, 
            ApiSpecificationPayload specification)
        {
            var path = $"{GetBasePath()}/apis/{apiName};rev={revision}?api-version=2018-06-01-preview";

            var response = await Constants.Azure
                .PutRestAsync(path, specification, (await TokenHelper.GetAzureAuthToken()).AddIfMatch());
        }

        private async static Task CreateOrUpdateAsync(ApiSpecificationPayload specification)
        {
            var path = $"{GetBasePath()}/apis/petstore?api-version=2018-06-01-preview";

            var response = await Constants.Azure
                .PutRestAsync(path, specification, (await TokenHelper.GetAzureAuthToken()).AddIfMatch());
        }

        private async static Task AddApiToProductAsync(string product, string apiName)
        {
            var path = $"{GetBasePath()}/products/{product}/apis/{apiName}?api-version=2018-06-01-preview";
            var response = await Constants.Azure
                    .PutRestAsync(path, string.Empty, await TokenHelper.GetAzureAuthToken());
        }

        private async static Task DeleteAsync(string apiId)
        {
            var path = $"{GetBasePath()}/apis/{apiId}?api-version=2018-06-01-preview";
            await Constants.Azure.DeleteRestAsync(path, (await TokenHelper.GetAzureAuthToken()).AddIfMatch());
        }

        private static async Task<ApiSpecificationCollection> ListAsync()
        {
            var path = "/apis?api-version=2018-06-01-preview";
            var apiSpecs = await Constants.APIMUri.GetRestAsync<ApiSpecificationCollection>(path, TokenHelper.GetAuthSasHeader(Constants.Sas));
            return apiSpecs;
        }

        private static string GetBasePath()
        {
            return $"/subscriptions/{Constants.SubscriptionId}/resourceGroups/{Constants.ResourceGroupName}/providers/Microsoft.ApiManagement/service/{Constants.ServiceName}";
        }
    }
}
