using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SimonPrinz.RequestLogger
{
	public static class RequestLoggerExtension
	{
		public static IServiceCollection AddRequestLogger(this IServiceCollection pServices)
		{
			pServices.TryAddScoped(typeof(RequestLoggerMiddleware));
			return pServices;
		}

		public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder pApp)
		{
			pApp.UseMiddleware<RequestLoggerMiddleware>();
			return pApp;
		}
	}
}