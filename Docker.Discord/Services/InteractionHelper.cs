using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Docker.Discord.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Docker.Discord.Services
{
	public sealed class InteractionHelper
	{
		private const string
			_apiUrl = "https://discord.com/api/v9",
			_commandUrl = "/applications/{application}/commands",
			_callbackUrl = "/interactions/{id}/{token}/callback",
			_originalResponseUrl = "/webhooks/{application}/{token}/messages/@original";
		
		private readonly IHttpClientFactory _factory;
		private readonly ulong _applicationId;
		private readonly string _authToken;
		
		private readonly AppCommand[] _commands = new[]
		{
			new AppCommand(null,"docker", "Docker-related commands.", new []
			{
				new AppCommandOption("command", "The command to execute", AppCommandOptionType.String, null, true, true),
				new AppCommandOption("arguments", "Optional arguments to pass", AppCommandOptionType.String, null, false)
			}),
			new AppCommand(null, "docker-compose", "Docker-compose related commands.", new[]
			{
				new AppCommandOption("command", "The command to execute", AppCommandOptionType.String, null, true, true),
				new AppCommandOption("arguments", "Optional arguments to pass", AppCommandOptionType.String, null, false)
			})
		};
		
		private ILogger<InteractionHelper> _logger;


		public InteractionHelper(IHttpClientFactory factory, ILogger<InteractionHelper> logger, IConfiguration config)
		{
			_factory = factory;
			_logger = logger;
			_applicationId = config.GetValue<ulong>("id");
			_authToken = config["token"];
		}

		public async Task RegisterCommandsAsync()
		{
			using var client = _factory.CreateClient();

			var payload = JsonConvert.SerializeObject(_commands);

			var request = new HttpRequestMessage(HttpMethod.Put, _apiUrl + _commandUrl.Replace("{application}", _applicationId.ToString()));

			request.Content = new StringContent(payload);
			
			request.Content.Headers.ContentType = new("application/json");
			request.Headers.TryAddWithoutValidation("Authorization", $"Bot {_authToken}");

			

			try
			{
				var res = await client.SendAsync(request);
				//TODO: Handle return response 
				res.EnsureSuccessStatusCode();
				
				_logger.LogInformation("Successfully registered slash commands.");
			}
			catch
			{
				_logger.LogCritical("Oh no something went wrong.");
			}
		}
	}
}