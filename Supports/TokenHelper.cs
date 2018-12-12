

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace apim_utils.Supports
{
    public static class TokenHelper
    {
       
        public static Action<HttpClient> GetAuthSasHeader(string token)
        {
            return new Action<HttpClient>((httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Authorization", token);
            });
        }

        public async static Task<Action<HttpClient>> GetAzureAuthToken()
        {
            var token = await GetAccessToken(
                Constants.TenantId, 
                Constants.ClientID, 
                Constants.ClientSecret,
                Constants.Azure.ToString());
            return GetAzureManagementApiHeaderFormatter(token);
        }

        public static async Task<AuthenticationResult>
            GetAccessToken(string tenantId, string clientId, string clientSecret, string resource = null)
        {
            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var authContext = new AuthenticationContext(authority);
            var token = await authContext.AcquireTokenAsync(
                resource,
                new ClientCredential(clientId, clientSecret));
            return token;
        }

        public static Action<HttpClient> AddIfMatch(this Action<HttpClient> action)
        {
            return new Action<HttpClient>((httpClient) =>
            {
                action(httpClient);

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", "*");
            });
        }

        public static Action<HttpClient> AddSpecialContentType(this Action<HttpClient> action, string contentType)
        {
            return new Action<HttpClient>((httpClient) =>
            {
                action(httpClient);

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue(contentType));
            });
        }

        public static Action<HttpClient> GetAzureManagementApiHeaderFormatter(
            this AuthenticationResult token)
        {
            return new Action<HttpClient>((httpClient) =>
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            });
        }
    }
}
