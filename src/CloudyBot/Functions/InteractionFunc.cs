using CloudyBot.Models;
using CloudyBot.Services;
using CloudyBot.Utilities;
using Discord.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;


namespace CloudyBot.Functions
{
    public class InteractionFunc
    {
        private readonly ICommandService _commandService;

        private List<string> gameServerCommandNames = new List<string>()
            { "server-list", "start", "stop", "stop-all", "delete", "delete-all" };

        private List<string> clientSetupCommandNames = new List<string>()
            { "setup", "add" };

        public InteractionFunc(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [FunctionName("InteractionFunc")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("gameserver-queue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> gameServerQueue,
            [Queue("client-setup-queue"), StorageAccount("AzureWebJobsStorage")] ICollector<string> clientSetupQueue,
            ILogger log)
        {
            // Workaround for a null reference exception thrown when serializing these properties.
            // I think it's a Discord.NET bug because they're derived properties and shouldn't be null.
            var resolver = new IgnorePropertiesResolver(new[] { "CommandName", "CommandId" });
            var settings = new JsonSerializerSettings() { ContractResolver = resolver };

            try
            {
                var command = await _commandService.ParseHttpCommandAsync(req);

                switch(command.Type)
                {
                    case CommandType.Ping:
                        return new JsonContentResult(command.AcknowledgePing());

                    case CommandType.Command:
                        var serializedCommand = JsonConvert.SerializeObject(command, settings);

                        if (gameServerCommandNames.Contains(command.Name))
                        {
                            gameServerQueue.Add(serializedCommand);
                        }
                        if (clientSetupCommandNames.Contains(command.Name))
                        {
                            clientSetupQueue.Add(serializedCommand);
                        }

                        return new JsonContentResult(command.Defer());

                    default:
                        throw new NotSupportedException("Invalid command type");
                }
            }
            catch (BadSignatureException ex)
            {
                log.LogError(ex.ToString());
                return new UnauthorizedResult();
            }
            catch (NotSupportedException ex)
            {
                log.LogError(ex.ToString());
                return new BadRequestObjectResult(ex.ToString());
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new ExceptionResult(ex, true);
            }
        }
    }
}
