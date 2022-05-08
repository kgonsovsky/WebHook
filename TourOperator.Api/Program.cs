using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;

namespace TourOperator.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseUrls("http://0.0.0.0:777")
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddJsonFile(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false);
            })
            .UseStartup<Startup>();

}