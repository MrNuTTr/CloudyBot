using CloudyBot.Models;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudyBot.Services
{
    public class DiscordService : ICommandService
    {
        private readonly string _publicKey;
        private readonly string _botToken;
        private readonly DiscordRestClient _discordClient;
        private readonly HttpClient _httpClient;

        public DiscordService(DiscordRestClient discordClient, HttpClient httpClient)
        {
            _publicKey = Environment.GetEnvironmentVariable("DISCORD_PUBLIC_KEY");
            _botToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            _discordClient = discordClient;
            _httpClient = httpClient;

            _discordClient.LoginAsync(TokenType.Bot, _botToken).Wait();
        }

        public async Task<ICommand> ParseHttpCommandAsync(HttpRequest req)
        {
            var signature = req.Headers["x-signature-ed25519"];
            var timestamp = req.Headers["x-signature-timestamp"];

            var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            reader.DiscardBufferedData();
            reader.Close();

            var interaction = await _discordClient.ParseHttpInteractionAsync(_publicKey, signature, timestamp, body);

            return new DiscordCommand(_httpClient, interaction, signature, timestamp, body);
        }

        public async Task<ICommand> ParseJsonStringCommandAsync(string jsonString)
        {
            dynamic json = JsonConvert.DeserializeObject(jsonString);

            var signature = json["Signature"].ToString();
            var timestamp = json["Timestamp"].ToString();
            var body = json["Body"].ToString();

            var interaction = await _discordClient.ParseHttpInteractionAsync(_publicKey, signature, timestamp, body);

            return new DiscordCommand(_httpClient, interaction, signature, timestamp, body);
        }
    }
}
