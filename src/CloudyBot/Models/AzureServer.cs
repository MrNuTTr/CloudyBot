using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Models
{
    public class AzureServer : CloudServer
    {
        private VirtualMachineResource _vmResource;
        private ArmClient _armClient;

        public AzureServer(string serverId, bool hasStaticPublicIp, string resourceId, ArmClient azureClient) : base(serverId, hasStaticPublicIp)
        {
            _armClient = azureClient;

            _vmResource = _armClient.GetVirtualMachineResource(new ResourceIdentifier(resourceId));
        }

        public override Task<bool> AttachPublicIpToServerAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> DeleteServerPublicIPAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> StartServerAsync()
        {
            var powerOnResponse = await _vmResource.PowerOnAsync(Azure.WaitUntil.Started);

            return powerOnResponse.HasCompleted;
        }

        public override Task<bool> StopServerAsync()
        {
            throw new NotImplementedException();
        }

        protected override bool LoadIsOnline()
        {
            var instanceView = _vmResource.InstanceView();
            var statuses = instanceView.Value.Statuses;

            foreach (var status in statuses)
            {
                if (status.Code == "PowerState/running")
                {
                    return true;
                }
            }

            return false;
        }

        protected override string LoadName()
        {
            return _vmResource.Data.Name;
        }

        protected override string LoadPublicIP()
        {
            throw new NotImplementedException();
        }
    }
}
