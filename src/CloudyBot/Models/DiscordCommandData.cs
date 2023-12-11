using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;

namespace CloudyBot.Models
{
    internal class DiscordCommandData : ICommandData
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public CommandDataType Type { get; set; }
        public IDictionary<string, ICommandData> Options { get; set; }

        public DiscordCommandData(RestSlashCommandData data)
        {
            Name = data.Name;
            Type = CommandDataType.None;
            Options = new Dictionary<string, ICommandData>();

            var enumerator = data.Options.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var key = enumerator.Current.Name;
                var value = new DiscordCommandData(enumerator.Current);

                Options.Add(key, value);
            }
        }

        public DiscordCommandData(RestSlashCommandDataOption option)
        {
            Name = option.Name;
            Value = option.Value;
            Type = option.Type switch
            {
                ApplicationCommandOptionType.String => CommandDataType.String,
                ApplicationCommandOptionType.Integer => CommandDataType.Integer,
                ApplicationCommandOptionType.Boolean => CommandDataType.Boolean,
                _ => throw new NotSupportedException($"Unsupported application type: {option.Type}")
            };
        }
    }
}
