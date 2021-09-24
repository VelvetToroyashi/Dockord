using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Docker.Discord
{
	public class RequestLoggerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestLoggerMiddleware> _logger;

		public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
		{
			_next = next;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next.Invoke(context);
			}
			finally
			{
				//_logger.LogInformation("Request to {Method} {Url} returned {Status}-{Name} Body: {Body}",context.Request.Method, context.Request.Path, context.Response.StatusCode, (HttpStatusCode)context.Response.StatusCode, new StreamReader(context.Request.Body).ReadToEndAsync().GetAwaiter().GetResult());
			}
			
		}
	}
}