using Azure.Data.Tables;
using CloudyBot.Models;
using CloudyBot.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Services
{
    public enum CloudServiceType : uint
    {
        Azure,
        DigitalOcean
    }

    public abstract class CloudService
    {
        protected string _clientId;
        protected TableClient _serverDataTable;

        public static CloudService CreateFromClientData(TableClient serverDataTable, ClientData clientData)
        {
            return clientData.CloudService switch
            {
                CloudServiceType.Azure => new AzureService(serverDataTable, clientData.PartitionKey, clientData.TenantID),
                _ => throw new NotSupportedException($"Unsupported cloud service type: {clientData.CloudService}")
            };
        }

        public CloudService(TableClient serverDataTable, string clientId) 
        { 
            _serverDataTable = serverDataTable;
            _clientId = clientId;
        }

        public abstract CloudServer GetCloudServer(string serverId);
    }
}
