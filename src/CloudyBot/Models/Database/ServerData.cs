using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Models.Database
{
    public class ServerData : ITableEntity
    {
        /// <summary>
        /// Acts as ClientID
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// Acts as ServerID
        /// </summary>
        public string RowKey { get; set; }
        /// <summary>
        /// Virtual Machine resource identifier. Azure specific.
        /// </summary>
        public string ResourceID { get; set; }
        public bool HasStaticPublicIP { get; set; }
        public string PublicIP { get; set; }
        public int MaxOnlineTimeHours { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
