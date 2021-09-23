using System.IO;
using System.Threading.Tasks;
using Docker.Discord.Services;
using Microsoft.AspNetCore.Mvc;

namespace Docker.Discord.Controllers
{
	[ApiController]
	[Route("api/v1")]
	public class InteractionsController : ControllerBase
	{
		private readonly string _key = "Key here";
		
		[HttpPost]
		[Route("interactions")]
		public async Task<IActionResult> HandleInteractionAsync() /* TODO: InteractionPayload payload*/
		{
			if (!HeaderHelpers.HasRequisiteHeaders(Request.Headers, out var ts, out var si))
				return Unauthorized();

			using var bodyReader = new StreamReader(Request.Body);
			var body = await bodyReader.ReadToEndAsync();

			if (!HeaderHelpers.ValidateHeaderSignature(ts, body, si, _key))
				return Unauthorized();


			return Accepted();
		}
	}
}