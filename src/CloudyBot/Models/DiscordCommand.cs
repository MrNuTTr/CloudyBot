using Discord.Rest;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudyBot.Models
{
    public class DiscordCommand : ICommand
    {
        private readonly RestInteraction _interaction;
        private readonly HttpClient _httpClient;

        public string Signature { get; set; }
        public string Timestamp { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
        public CommandType Type { get; set; }
        public ICommandData Data { get; set; }

        public DiscordCommand(HttpClient httpClient, RestInteraction interaction, string signature, string timestamp, string body)
        {
            _httpClient = httpClient;

            _interaction = interaction;
            Signature = signature;
            Timestamp = timestamp;
            Body = body;

            if (interaction is RestPingInteraction)
            {
                Type = CommandType.Ping;
            }
            else if (interaction is RestSlashCommand)
            {
                var command = interaction as RestSlashCommand;

                Name = command.Data.Name;
                Type = CommandType.Command;
                Data = new DiscordCommandData(command.Data);
            }
            else
            {
                throw new NotSupportedException($"Unrecognized interaction type: {interaction.Type}");
            }
        }

        /// <summary>
        /// Get the ID of the client this command is associated with.
        /// </summary>
        /// <returns></returns>
        public string GetClientID()
        {
            return _interaction.GuildId.ToString();
        }

        public string AcknowledgePing()
        {
            return (_interaction as RestPingInteraction).AcknowledgePing();
        }

        public string Defer()
        {
            return _interaction.Defer();
        }

        public async Task FollowupAsync(string content)
        {
            await _interaction.FollowupAsync(content);
        }

        public async Task RespondAsync(string content)
        {
            string baseUrl = "https://discord.com/api/webhooks";
            string url = $"{baseUrl}/{_interaction.ApplicationId}/{_interaction.Token}";

            var jsonData = JsonConvert.SerializeObject(content);

            var contentData = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, contentData);

            response.EnsureSuccessStatusCode();
        }
    }
}
