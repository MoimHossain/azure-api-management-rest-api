using System;
using System.Collections.Generic;
using System.Text;

namespace apim_utils
{
    public class Constants
    {
        public static Uri Azure = new Uri("https://management.azure.com");
        public const string ServiceName = "";
        public static string ResourceGroupName = "";
        public const string SubscriptionId = "";
        public const string Sas = "";
        public const string TenantId = "";
        public const string ClientID = "";
        public const string ClientSecret = "";

        public const string ApiTenantName = "";
        public static Uri APIMUri = new Uri($"https://{ApiTenantName}.management.azure-api.net");
    }
}
