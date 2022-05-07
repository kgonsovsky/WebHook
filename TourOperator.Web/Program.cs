using Microsoft.AspNetCore;

namespace TourOperator.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);
                })
                .UseStartup<Startup>();
    }
}
