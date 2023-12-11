using Azure;
using Azure.Data.Tables;
using Azure.ResourceManager.Network;
using CloudyBot.Models;
using CloudyBot.Models.Database;
using CloudyBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudyBot.Functions
{
    public class GameServerFunc
    {
        private readonly ICommandService _commandService;
        private CloudService _cloudService;
        private ILogger _log;

        private string _serverId;
        private string _clientId;
        private ICommand _command;
        private ClientData _clientData;
        private ServerData _serverData;
        private TableClient _shutdownRequestTable;
        private TableClient _clientDataTable;
        private TableClient _serverDataTable;

        public GameServerFunc(ICommandService commandService)
        {
            this._commandService = commandService;
        }

        [FunctionName("GameServerFunc")]
        public async Task Run(
            [QueueTrigger("gameserver-queue", Connection = "AzureWebJobsStorage")] string queueMessage,
            [Table("shutdownRequests")] TableClient shutdownRequestTable,
            [Table("clientData")] TableClient clientDataTable,
            [Table("serverData")] TableClient serverDataTable,
            ILogger logger)
        {
            _shutdownRequestTable = shutdownRequestTable;
            _clientDataTable = clientDataTable;
            _serverDataTable = serverDataTable;
            _log = logger;

            _command = await _commandService.ParseJsonStringCommandAsync(queueMessage);
            _clientId = _command.GetClientID();

            try
            {
                _clientData = clientDataTable.GetEntity<ClientData>(_clientId, _clientId).Value;
                _cloudService = CloudService.CreateFromClientData(serverDataTable, _clientData);

                _serverId = _command.Data.Options["server"].Value.ToString();
                _serverData = serverDataTable.GetEntity<ServerData>(_clientId, _serverId).Value;
                
                switch (_command.Name)
                {
                    case "server-list":
                        throw new NotImplementedException();
                    case "start":
                        await StartAsync();
                        return;
                    case "stop":
                        throw new NotImplementedException();
                    case "stop-all":
                        throw new NotImplementedException();
                    case "delete":
                        throw new NotImplementedException();
                    case "delete-all":
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
            catch(RequestFailedException ex)
            {
                await _command.FollowupAsync(
                    "Whoops, looks like you haven't used `/setup` yet.\n" +
                    "Or maybe the server was deleted? Try `/add`.");
                _log.LogError(ex.Message);
            }
            catch(Exception ex)
            {
                await _command.FollowupAsync("Okay, that's weird, not sure what happened. Try again or reach out to support.");
                _log.LogError(ex.Message);
            }
        }

        public async Task StartAsync()
        {
            CloudServer server;

            var shutdownRequest = new ShutdownRequest
            {
                PartitionKey = _clientId,
                RowKey = _serverId,
                NextShutdownTimestamp = DateTime.UtcNow.AddHours(_serverData.MaxOnlineTimeHours)
            };

            await _command.FollowupAsync("Turning on server. Please wait a minute.");

            try
            {
                server = _cloudService.GetCloudServer(_serverId);
            }
            catch(Exception ex)
            {
                _log.LogError(ex.Message);
                await _command.RespondAsync($"Couldn't find server {_serverId}. Did it get deleted?");
                return;
            }

            try
            {
                _shutdownRequestTable.AddEntity(shutdownRequest);
            }
            catch(RequestFailedException)
            {
                // Throws error if the request doesn't exist
                var shutdownEntity = _shutdownRequestTable.GetEntity<ShutdownRequest>(_clientId, _serverId).Value;

                if (server.IsOnline)
                {
                    await _command.RespondAsync($"Server is already online. IP: `{server.PublicIP}`");
                    return;
                }
            }
            catch(Exception ex)
            {
                _log.LogError($"Error starting server {_serverId}\n" +
                    $"Message: {ex.Message}\n" +
                    $"Stack trace: \n{ex.StackTrace}");

                await _command.RespondAsync($"Couldn't start server for some reason. Technical error message: {ex.Message}");

                return;
            }

            try
            {
                await server.StartServerAsync();

                await _command.RespondAsync(
                        $"Server is now online and will turn off in {_serverData.MaxOnlineTimeHours}. " +
                        $"IP is: `{server.PublicIP}`");
            }
            catch (Exception ex)
            {
                _log.LogError($"Couldn't turn on server: {ex.Message}");
                await _command.RespondAsync($"Couldn't turn on server for some reason. " +
                    $"Technical error message: {ex.Message}");
            }
        }
    }
}
