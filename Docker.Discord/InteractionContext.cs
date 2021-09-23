using DSharpPlus;

namespace Docker.Discord
{
	public class InteractionContext
	{
		private readonly DiscordRestClient _rest;
		private readonly string _token;
		private readonly ulong _id;

		internal InteractionContext(ulong id, string token, DiscordRestClient client)
		{
			_id = id;
			_token = token;
			_rest = client;
		}
		
		
	}
}