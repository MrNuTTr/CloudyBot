using Azure;
using Azure.Data.Tables;
using CloudyBot.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITableEntity = Azure.Data.Tables.ITableEntity;

namespace CloudyBot.Models.Database
{
    

    public class ClientData : ITableEntity
    {
        /// <summary>
        /// Enum raw value. Used for parsing, don't use directly.
        /// </summary>
        public int CloudServiceValue { get; set; }

        /// <summary>
        /// This acts as the ClientID.
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// This acts as the ClientID.
        /// </summary>
        public string RowKey { get; set; }
        /// <summary>
        /// The TenantID for Azure specific environments.
        /// </summary>
        public string TenantID { get; set; }
        /// <summary>
        /// Specifies cloud service this client uses.
        /// </summary>
        [IgnoreProperty]
        public CloudServiceType CloudService 
        {
            get { return (CloudServiceType)CloudServiceValue; } 
        }
        
        
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
