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

	public record InteractionResponsePayload (
		[property: JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseType Type,
		[property: JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)] InteractionResponseBuilder Data
		);
}