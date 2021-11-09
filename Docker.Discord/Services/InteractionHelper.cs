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
using Newtonsoft.Json.Serialization;
using FuzzySharp;

namespace Docker.Discord.Services;

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
			name = "docker",
			type = 1,
			description = "Docker-related commands",
			options = new[] 
			{
				new 
				{
					name = "command",
					type = 3,
					description = "The command to execute",
					required = true,
					autocomplete = true
				},
				new 
				{ 
					name = "args", 
					type = 3,
					description = "Arguments to pass to the command",
					required = false,
					autocomplete = true
				}
			}
		},
		new
		{ 
			name = "docker-compose",
			type = 1,
			description = "Docker-Compose-related commands",
			options = new[] 
			{
				new 
				{
					name = "command",
					type = 3,
					description = "The command to execute",
					required = true,
					autocomplete = true
				},
				new 
				{ 
					name = "args", 
					type = 3,
					description = "Arguments to pass to the command",
					required = false,
					autocomplete = true
				}
			}
		},
	};
	
	private readonly string[] _dockerCommands =
    {
        "exec", "images", "info",
        "inspect", "kill", "logs",
        "network", "ps", "pull",
        "rename", "restart", "rm",
        "rmi", "run", "start",
        "stop", "update", "version"
    };
	
	private readonly string[] _dockerComposeCommands =
    {
        "build", "config", "create",
        "down", "events", "images", 
        "logs", "pull", "restart",
        "rm", "run", "scale",
        "start", "stop", "up"
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


		var t = obj["data"]["name"].ToString() switch
		{
			"docker" => SuggestDockerAsync(obj),
			"docker-compose" => SuggestDockerComposeAsync(obj),
			_ => throw new ArgumentException($"Unknown command invocation: {obj}")
		};
		
		await t;
	}
	
	private async Task SuggestDockerAsync(JObject input)
	{
		var id = input["id"].ToObject<ulong>();
		var token = input["token"].ToString();
		
		var command = input["data"]["options"][0]["value"].ToString();

		if (string.IsNullOrEmpty(command))
		{
			var payload =
				new
				{
					type = 8,
					data = new 
					{
						choices = _dockerCommands.Select(x => new { name = x, value = x })
					}
				};
			var response = JsonConvert.SerializeObject(payload);

			var request = GeneratePayloadRequest(response, "/callback", id, token);
			
		    await _client.SendAsync(request);
		}
		else
		{
			var selections = FuzzySharp.Process.ExtractTop(command, _dockerCommands, limit: 10, cutoff: 60);
			var payload = new
			{
				type = 8,
				data = new
	            {
	                choices = selections.Select(x => new { name = x.Value, value = x.Value })
	            }
			};
			var response = JsonConvert.SerializeObject(payload);
			
			var request = GeneratePayloadRequest(response, _callbackUrl, id, token);
	        
			await _client.SendAsync(request);
		}
	}
	
	private async Task SuggestDockerComposeAsync(JObject input)
	{
		var id = input["id"].ToObject<ulong>();
		var token = input["token"].ToString();
        
		var command = input["data"]["options"][0]["value"].ToString();
		
		if (string.IsNullOrEmpty(command))
        {
            var payload =
                new
                {
                    type = 8,
                    data = new 
                    {
                        choices = _dockerComposeCommands.Select(x => new { name = x, value = x })
                    }
                };
            var response = JsonConvert.SerializeObject(payload);

            var request = GeneratePayloadRequest(response, "/callback", id, token);
            
            await _client.SendAsync(request);
        }
        else
        {
            var selections = FuzzySharp.Process.ExtractTop(command, _dockerComposeCommands, limit: 10, cutoff: 60);
            var payload = new
            {
                type = 8,
                data = new
                {
                    choices = selections.Select(x => new { name = x.Value, value = x.Value })
                }
            };
            var response = JsonConvert.SerializeObject(payload);
            
            var request = GeneratePayloadRequest(response, _callbackUrl, id, token);
            
            await _client.SendAsync(request);
        }
	}


	private HttpRequestMessage GeneratePayloadRequest(string payload, string endpoint, ulong id, string token)
	{
		var request = new HttpRequestMessage(HttpMethod.Post, (_apiUrl + endpoint)
			.Replace("{id}", id.ToString())
			.Replace("{token}", token));
		
		_logger.LogTrace("Payload: {Payload}", payload);
		
		request.Content = new StringContent(payload);
		
		request.Content.Headers.ContentType = new("application/json");
		request.Headers.TryAddWithoutValidation("Authorization", $"Bot {_authToken}");

		return request;
	}
}
