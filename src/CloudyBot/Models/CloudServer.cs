using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Models
{
    public abstract class CloudServer
    {
        private string _name;
        private string _publicIp;
        private bool _isOnline;

        public string Name 
        {
            get
            {
                if (_name == null)
                {
                    _name = LoadName();
                }
                return _name;
            }
        }
        public string PublicIP 
        {
            get
            {
                if (_publicIp == null)
                {
                    _publicIp = LoadPublicIP();
                }
                return _publicIp;
            }
        }
        public bool IsOnline 
        { 
            get
            {
                return LoadIsOnline();
            }
        }
        public string ID { get; }
        public bool HasStaticPublicIP { get; }

        public CloudServer(string serverId, bool hasStaticPublicIp) 
        {
            ID = serverId;
            HasStaticPublicIP = hasStaticPublicIp;
        }

        public abstract Task<bool> StartServerAsync();
        public abstract Task<bool> StopServerAsync();
        public abstract Task<bool> AttachPublicIpToServerAsync();
        public abstract Task<bool> DeleteServerPublicIPAsync();

        protected abstract string LoadName();
        protected abstract string LoadPublicIP();
        protected abstract bool LoadIsOnline();
    }
}
