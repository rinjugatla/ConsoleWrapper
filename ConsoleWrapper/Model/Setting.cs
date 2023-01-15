using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConsoleWrapper.Model
{
    public partial class Setting
    {
        [JsonProperty("app")]
        public App App { get; set; }

        [JsonProperty("basic_commands")]
        public List<BasicCommand> BasicCommands { get; set; }

        [JsonProperty("macro_commands")]
        public List<MacroCommand> MacroCommands { get; set; }
    }

    public partial class App
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class BasicCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("command")]
        public Command Command { get; set; }

        [JsonProperty("command_patterns", NullValueHandling = NullValueHandling.Ignore)]
        public List<CommandPattern> CommandPatterns { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public partial class CommandPattern
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pattern", NullValueHandling = NullValueHandling.Ignore)]
        public string Pattern { get; set; }
    }

    public partial class MacroCommand
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("commands")]
        public List<Command> Commands { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public partial class Command
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }
    }

    public partial class Setting
    {
        public static List<Setting> FromJson(string json) => JsonConvert.DeserializeObject<List<Setting>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<Setting> self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
