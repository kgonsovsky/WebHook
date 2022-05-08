using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourOperator.Api.Controllers;
using TourOperator.Api.Services;
using TourOperator.Db;

namespace TourOperator.Api;

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
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")
                    , builder =>
                    {
                        builder.MigrationsAssembly("TourOperator.Db");
                    }
                );

            }
        );
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddRazorPages();
        services.AddHttpClient();

        services.AddTransient<Publisher>();
        services.AddScoped<GateWayController>();

        services.AddSingleton<Processor>();
        services.AddSingleton<IHostedService, Processor>(
            serviceProvider =>
            {
                var p = (Processor)serviceProvider.GetService<Processor>();
                return p;
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}