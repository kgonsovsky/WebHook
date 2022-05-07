using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;

namespace TourOperator.Web
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

            services.AddMvc(
                a=> a.EnableEndpointRouting=false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app)
        {

                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
 
        
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Subscription}/{action=Index}/{id?}");
            });
        }
    }
}
