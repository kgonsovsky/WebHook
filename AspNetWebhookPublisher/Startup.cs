using AspNetWebhookPublisher.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetWebhookPublisher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                
            }, ServiceLifetime.Singleton);
            services.AddHttpClient();
            services.AddSingleton<WebhookPublisher>();
            services.AddSingleton<IHostedService, WebhookPublisher>(
                serviceProvider => serviceProvider.GetService<WebhookPublisher>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            dbContext.Database.Migrate();
        }
    }
}
