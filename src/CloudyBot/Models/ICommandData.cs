using System.Collections.Generic;

namespace CloudyBot.Models
{
    public enum CommandDataType
    {
        None,
        String,
        Integer,
        Boolean
    }

    public interface ICommandData
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public CommandDataType Type { get; set; }
        public IDictionary<string, ICommandData> Options { get; set; }
    }
}
