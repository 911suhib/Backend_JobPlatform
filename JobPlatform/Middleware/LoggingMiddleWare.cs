
namespace JobPlatformBackend.API.Middleware
{
	public class LoggingMiddleWare : IMiddleware
	{
		private readonly ILogger _logger;
		public LoggingMiddleWare(ILogger<LoggingMiddleWare> logger)
		{
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			var originalResposeBody = context.Response.Body;

			using var resposeBodyStream = new MemoryStream();
			context.Response.Body = resposeBodyStream;

			try
			{
				_logger.LogInformation(
									"{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - Request:{Method} {Path}{QueryString}, User-Agent: {UserAgent}, Remote IP: {RemoteIpAddress}",
									DateTime.Now,
									context.Request.Method,
									context.Request.Path,
									context.Request.QueryString,
									context.Request.Headers["User-Agent"],
									context.Connection.RemoteIpAddress
								);
				await next(context);
				resposeBodyStream.Seek(0, SeekOrigin.Begin);
				var responseBody = await new StreamReader(resposeBodyStream).ReadToEndAsync();
				_logger.LogInformation("Response: {StatusCode}, Content: {ResponseBody}",
				context.Response.StatusCode,
				responseBody);
				resposeBodyStream.Seek(0, SeekOrigin.Begin);
				await resposeBodyStream.CopyToAsync(originalResposeBody);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing the request.");
				throw;
			}
			finally
			{
				context.Response.Body = originalResposeBody; 
			}
		}
	}
}
