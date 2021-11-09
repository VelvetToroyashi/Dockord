using System;
using System.IO;
using System.Threading.Tasks;
using Docker.Discord.Services;
using Docker.Discord.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Docker.Discord.Controllers
{
	[ApiController]
	[Route("api/v1")]
	public class InteractionsController : ControllerBase
	{
		private readonly string _key;
		private readonly ILogger<InteractionsController> _logger;
		private readonly InteractionHelper _interactions;
		public InteractionsController(IConfiguration config, InteractionHelper interactions, ILogger<InteractionsController> logger)
        {
            _key = config.GetValue<string>("key");
            _interactions = interactions;
            _logger = logger;
        }

		[HttpPost]
		[Route("interactions")]
		public async Task<IActionResult> HandleInteractionAsync() /* TODO: InteractionPayload payload*/
		{
			var now = DateTimeOffset.UtcNow;
			using var bodyReader = new StreamReader(Request.Body);
			var body = await bodyReader.ReadToEndAsync();
			
			
			if (!HeaderHelpers.HasRequisiteHeaders(Request.Headers, out var ts, out var si))
			{
				_logger.LogWarning("Missing headers");
				return Unauthorized();
			}

			if (!HeaderHelpers.ValidateHeaderSignature(ts, body, si, _key))
			{
				_logger.LogWarning("Invalid signature");
				return Unauthorized();
			}
			
			var bodyObj = JObject.Parse(body);

			if (bodyObj["type"]?.ToObject<InteractionType>() is InteractionType.Ping)
			{
				_logger.LogInformation("Ping received");
				return Ok(new InteractionResponsePayload(InteractionResponseType.Pong));
			}

			await _interactions.HandleInteractionAsync(bodyObj, now);
			
			return Accepted();
		}
	}
}