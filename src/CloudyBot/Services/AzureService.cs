using Azure.Data.Tables;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using CloudyBot.Models;
using CloudyBot.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Services
{
    public class AzureService : CloudService
    {
        private readonly string _appSecret = Environment.GetEnvironmentVariable("AZURE_APP_SECRET");
        private readonly string _appId = Environment.GetEnvironmentVariable("AZURE_APP_ID");
        private ArmClient _client { get; set; }

        public AzureService(TableClient serverDataTable, string clientId, string tenantId) : base(serverDataTable, clientId)
        {
            var options = new ClientSecretCredentialOptions
            {
                AdditionallyAllowedTenants = { "*" }
            };

            var credential = new ClientSecretCredential(tenantId, _appId, _appSecret, options);

            _client = new ArmClient(credential);
        }

        public override CloudServer GetCloudServer(string serverId)
        {
            var data = _serverDataTable.GetEntity<ServerData>(_clientId, serverId).Value;

            return new AzureServer(serverId, data.HasStaticPublicIP, data.ResourceID, _client);
        }
    }
}
