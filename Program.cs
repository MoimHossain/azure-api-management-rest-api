

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using apim_utils.Models;
using System.Linq;
using apim_utils.Supports;
using apim_utils.Experiments;

namespace apim_utils
{
    /// <summary>
    /// apim_utils -cerate-revision "
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            RunAppAsync().Wait();
        }        

        private async static Task RunAppAsync()
        {
            var apim = await ApimClient.BuildAsync();
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

            await apim.CreateOrUpdateAsync(apiName, petStoreApiSpec);

            await apim.AddApiToProductAsync(product, apiName);

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
            await apim.AddRevisionAsync(apiName, "preview", betaSpec);


            await apim.ReleaseRevisionAsync(apiName, "preview", "Release this revision");
        }

    }
}
