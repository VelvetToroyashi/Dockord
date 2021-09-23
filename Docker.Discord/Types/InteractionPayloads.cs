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
	
	public record InteractionResponsePayload (
		[property: JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseType Type,
		[property: JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseBuilder Data
		);

	public sealed record InboundInteractionPayload()
	{
		public int Id { get; init; }
		
		public ulong ApplicationId { get; init; }
		
		public InteractionType Type { get; init; }
		
		public InteractionData Data { get; init; }

		public ulong? GuildId { get; init; }

		public ulong? ChannelId { get; init; }

		public string Token { get; init; }
	}

	public sealed record InteractionData
	{
		public string Name { get; init; }
		
		public string CustomId { get; init; }
		
		public ComponentType ComponentType { get; init; }
	}
	
	
}