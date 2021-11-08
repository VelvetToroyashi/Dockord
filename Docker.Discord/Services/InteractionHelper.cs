using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Docker.Discord.Types;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Docker.Discord.Services
{
	public sealed class InteractionHelper
	{
		private const string
			_apiUrl = "https://discord.com/api/v9",
			_commandUrl = "/applications/{application}/guilds/721518523704410202/commands",
			_callbackUrl = "/interactions/{id}/{token}/callback",
			_originalResponseUrl = "/webhooks/{application}/{token}/messages/@original";
		
		private readonly HttpClient _client;
		private readonly ulong _applicationId;
		private readonly string _authToken;

		private readonly object[] _commandList =
		{
			new
			{ 
				Name = "docker",
				Description = "Docker-related commands",
				Options = new[] 
				{
					new 
					{
						Name = "command",
						Description = "The command to execute",
						Type = 3,
						Required = true,
						AutoComplete = true
					},
					new 
					{ 
						Name = "args", 
						Description = "Arguments to pass to the command",
						Type = 3,
						Required = false,
						AutoComplete = true
					}
				}
			},
			new
			{
				Name = "docker-compose",
				Description = "Docker-Compose related commands",
				Options = new[] 
                {
                    new 
                    {
                        Name = "command",
                        Description = "The command to execute",
                        Type = 3,
                        Required = true,
                        AutoComplete = true
                    },
                    new 
                    { 
                        Name = "args", 
                        Description = "Arguments to pass to the command",
                        Type = 3,
                        Required = false,
                        AutoComplete = true
                    }
                }
			}
		};
		
		private ILogger<InteractionHelper> _logger;
	
		
		
		public InteractionHelper(HttpClient client, ILogger<InteractionHelper> logger, IConfiguration config)
		{
			_client = client;
			_logger = logger;
			_applicationId = config.GetValue<ulong>("id");
			_authToken = config["token"];
		}

		public async Task RegisterCommandsAsync()
		{
			var payload = JsonConvert.SerializeObject(_commandList);
			var request = new HttpRequestMessage(HttpMethod.Put, _apiUrl + _commandUrl.Replace("{application}", _applicationId.ToString()));

			request.Content = new StringContent(payload);
			
			request.Content.Headers.ContentType = new("application/json");
			request.Headers.TryAddWithoutValidation("Authorization", $"Bot {_authToken}");

			HttpResponseMessage res = await _client.SendAsync(request);
			try
			{ //TODO: Handle return response 
				res.EnsureSuccessStatusCode();
				
				_logger.LogInformation("Successfully registered slash commands.");
			}
			catch (HttpRequestException e)
			{
				var responseBody = await res.Content.ReadAsStringAsync();
				_logger.LogCritical("Discord returned 400 while registering slash commands. \n\nRegirstration JSON: \n{OutboundJSON} \n\nReturned JSON: \n{InboundJSON}", payload, responseBody);
			}
		}

		public async Task HandleInteractionAsync(JObject obj, DateTimeOffset then)
		{
			var now = DateTimeOffset.UtcNow;
			_logger.LogTrace("Proccessing time: {Time}ms", (now - then).TotalMilliseconds);
			
			
			
		}


		private HttpRequestMessage GeneratePayloadRequest(string payload, string endpoint, ulong id, string token)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, (_apiUrl + _callbackUrl)
				.Replace("{id}", id.ToString())
				.Replace("{token}", token));
			
			_logger.LogWarning("Payload: {Payload}", payload);
			
			request.Content = new StringContent(payload);
			
			request.Content.Headers.ContentType = new("application/json");
			request.Headers.TryAddWithoutValidation("Authorization", $"Bot {_authToken}");

			return request;
		}
	}
}