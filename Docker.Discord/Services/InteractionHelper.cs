using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Docker.Discord.Services
{
	public sealed class InteractionHelper
	{
		private const string
			_apiUrl = "https://discord.com/api/v9",
			_callbackUrl = "/interactions/{id}/{token}/callback",
			_originalResponseUrl = "/webhooks/{application}/{token}/messages/@original";
		
		private readonly IHttpClientFactory _factory;
		private readonly ulong _applicationId;	
		
		public InteractionHelper(IHttpClientFactory factory, IConfiguration config)
		{
			_factory = factory;
			_applicationId = config.GetValue<ulong>("appid");
		}
		
		public async Task HandlePingAsync(ulong id, string token)
		{
			using var client = _factory.CreateClient();
			
			
		}
	
		
		
		
	}
}