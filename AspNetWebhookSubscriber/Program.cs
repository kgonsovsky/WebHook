using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AspNetWebhookSubscriber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);
            var builtHost = hostBuilder.Build();
            builtHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
            return builder;
        }
    }
}
