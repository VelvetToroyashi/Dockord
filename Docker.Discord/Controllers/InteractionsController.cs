using System.IO;
using System.Threading.Tasks;
using Docker.Discord.Services;
using Docker.Discord.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Docker.Discord.Controllers
{
	[ApiController]
	[Route("api/v1")]
	public class InteractionsController : ControllerBase
	{
		private readonly string _key = "key here";

		[HttpPost]
		[Route("interactions")]
		public async Task<IActionResult> HandleInteractionAsync() /* TODO: InteractionPayload payload*/
		{
			using var bodyReader = new StreamReader(Request.Body);
			var body = await bodyReader.ReadToEndAsync();
			
			if (!HeaderHelpers.HasRequisiteHeaders(Request.Headers, out var ts, out var si))
				return Unauthorized();
			
			if (!HeaderHelpers.ValidateHeaderSignature(ts, body, si, _key))
				return Unauthorized();
			
			var bodyObj = JObject.Parse(body);
			
			if (bodyObj["type"]?.ToObject<InteractionType>() is InteractionType.Ping)
				return Ok(new InteractionResponsePayload(InteractionResponseType.Pong));
			
			return Accepted();
		}
	}
}