using System.Net;

namespace JobPlatformBackend.API.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}
		public async Task Invoke(HttpContext context) {
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "حدث خطأ غير متوقع: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}
		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var response = new
			{
				StatusCode = context.Response.StatusCode,
				Message = "Internal Server Error من السيرفر، رح نصلحه حالاً!",
				Detail = exception.Message // ارفعه فقط في بيئة الـ Development
			};

			return context.Response.WriteAsJsonAsync(response);
		}
	}

}
