using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CloudyBot.Models
{
    public enum CommandType
    {
        Ping,
        Command,
    }

    public interface ICommand
    {
        public string Name { get; set; }
        public CommandType Type { get; set; }
        public ICommandData Data { get; set; }

        /// <summary>
        /// Get the database ID of the client who sent the command.
        /// </summary>
        public string GetClientID();
        
        /// <summary>
        /// Returns a json string payload acknowledging a ping from command provider.
        /// </summary>
        public string AcknowledgePing();
        
        /// <summary>
        /// Returns a json string payload telling command provider to wait for a further follow-up message.
        /// </summary>
        public string Defer();
        
        /// <summary>
        /// Sends a followup message to the command provider. Must have been deferred first.
        /// </summary>
        /// <param name="content">Message content seen by the user</param>
        public Task FollowupAsync(string content);

        /// <summary>
        /// Respond to a message that has already been followed up on. Acts as a reply to that message.
        /// </summary>
        /// <param name="content">Message content seen by the user</param>
        public Task RespondAsync(string content);
    }
}
