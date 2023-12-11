using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Models.Database
{
    public class ShutdownRequest : ITableEntity
    {
        /// <summary>
        /// Acts as ClientID
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// Acts as ServerID
        /// </summary>
        public string RowKey { get ; set ; }
        public DateTime NextShutdownTimestamp { get; set; }
        public DateTimeOffset? Timestamp { get ; set; }
        public ETag ETag { get; set; }
    }
}
