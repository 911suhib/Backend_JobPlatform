// File: API/Middleware/ExceptionMiddleware.cs
using JobPlatformBackend.Domain.src.Exceptions;
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

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "حدث خطأ: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";

 			var statusCode = (int)HttpStatusCode.InternalServerError;
			var message = "Internal Server Error من السيرفر، رح نصلحه حالاً!";
			object? additionalData = null;

 			if (exception is AppException appEx)
			{
				statusCode = appEx.StatusCode;
				message = appEx.Message;
				additionalData = appEx.AdditionalData; 
			}

			context.Response.StatusCode = statusCode;

			var response = new
			{
				StatusCode = statusCode,
				Message = message,
				Data = additionalData, 
				Detail = exception.Message  
			};

			return context.Response.WriteAsJsonAsync(response);
		}
	}
}