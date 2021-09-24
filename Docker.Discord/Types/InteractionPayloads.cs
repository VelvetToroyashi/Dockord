using Newtonsoft.Json;

namespace Docker.Discord.Types
{
	public enum InteractionResponseType
	{
		Pong = 1,
		SlashReply = 4,
		DeferredSlashReply = 5,
		ComponentACK = 6,
		ComponentUpdate = 7,
		AutoCompleteResponse = 8
	}

	public enum InteractionType
	{
		Ping = 1,
		AppCommand = 2,
		MessageComponent = 3,
		AutoCompleteRequest = 4,
	}

	public enum ComponentType
	{
		ActionRow = 1,
		Button = 2,
		Dropdown = 3
	}

	public enum AppCommandOptionType
	{
		SubCommand = 1,

		SubCommandGroup,

		String,

		Integer,

		Boolean,

		User,

		Channel,

		Role,

		Mentionable,

		FloatingPoint
	}

	public enum AppCommandType
	{
		SlashCommand = 1,
		UserContextMenu = 2,
		MessageContextMenu = 3,
		AutoCompleteRequest = 4,
	}

	public record InteractionResponsePayload(
		[property: JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseType Type,
		[property: JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseBuilder Data = null
	);

	public sealed record InboundInteractionPayload
	{
		[JsonProperty("id")]
		public ulong Id { get; init; }

		[JsonProperty("application_id")]
		public ulong ApplicationId { get; init; }

		[JsonProperty("type")]
		public InteractionType Type { get; init; }

		[JsonProperty("data")]
		public InteractionData Data { get; init; }

		[JsonProperty("guild_id")]
		public ulong? GuildId { get; init; }

		[JsonProperty("channel_id")]
		public ulong? ChannelId { get; init; }

		[JsonProperty("token")]
		public string Token { get; init; }
	}

	public sealed record InteractionData
	{
		[JsonProperty("name")]
		public string Name { get; init; }

		[JsonProperty("custom_id")]
		public string CustomId { get; init; }
		
		[JsonProperty("type")]
		public ComponentType ComponentType { get; init; }
		
		[JsonProperty("options")]
		public InteractionDataOption[] Options { get; init; }
	}


	public sealed record InteractionDataOption
	{
		[JsonProperty("name")]
		public string Name { get; init; }
		
		[JsonProperty("type")]
		public AppCommandOptionType Type { get; init; }
		
		[JsonProperty("focused")]
		public bool Focused { get; init; }
		
		[JsonProperty("value")]
		public string RawValue { get; init; }

		public object Value => Type switch
		{
			AppCommandOptionType.Boolean => bool.Parse(RawValue),
			AppCommandOptionType.Integer => long.Parse(RawValue),
			AppCommandOptionType.String => RawValue,
			AppCommandOptionType.Channel => ulong.Parse(RawValue),
			AppCommandOptionType.User => ulong.Parse(RawValue),
			AppCommandOptionType.Role => ulong.Parse(RawValue),
			AppCommandOptionType.Mentionable => ulong.Parse(RawValue),

			_ => RawValue.ToString()
		};

		[JsonProperty("options")]
		public InteractionDataOption Options { get; init; }
	}
	
	
	public sealed record AppCommand(
		[property: JsonProperty("id")] ulong? Id,
		[property: JsonProperty("name")] string Name,
		[property: JsonProperty("description")] string Description,
		[property: JsonProperty("options")] AppCommandOption[]? Options,
		[property: JsonProperty("default_permission")] bool? DefaultPermission = null,
		[property: JsonProperty("type")] AppCommandType Type = AppCommandType.SlashCommand);

	public sealed record AppCommandOption(
		[property: JsonProperty("name")] string Name, 
		[property: JsonProperty("description")] string Description, 
		[property: JsonProperty("type")] AppCommandOptionType Type, 
		[property: JsonProperty("choices")] AppCommandChoice[]? Choices,
		[property: JsonProperty("required")] bool? Required = null, 
		[property: JsonProperty("autocomplete")] bool? AutoComplete = null);

	public sealed record AppCommandChoice(
		[property: JsonProperty("name")] string Name, 
		[property: JsonProperty("value")] object Value);
	
}
