using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SimonPrinz.RequestLogger.Sample
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection pServices)
		{
			pServices.AddRequestLogger();
		}

		public void Configure(IApplicationBuilder pApp, IWebHostEnvironment pEnv)
		{
			pApp.UseRequestLogger();

			if (pEnv.IsDevelopment())
				pApp.UseDeveloperExceptionPage();

			pApp.UseRouting();
			pApp.UseEndpoints(pEndpoints =>
				pEndpoints.MapGet("/", async pContext =>
					await pContext.Response.WriteAsync("Hello World!")));
		}
	}
}