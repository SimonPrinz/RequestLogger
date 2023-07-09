using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SimonPrinz.RequestLogger.Sample
{
	public static class Program
	{
		public static void Main(string[] pArgs) =>
			Host.CreateDefaultBuilder(pArgs)
				.ConfigureWebHostDefaults(pBuilder =>
					pBuilder.UseStartup<Startup>())
				.Build()
				.Run();
	}
}