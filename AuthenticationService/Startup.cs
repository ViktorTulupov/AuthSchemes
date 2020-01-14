using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationService.Filters;
using AuthenticationService.Services;
using Microsoft.Extensions.Hosting;
using AuthenticationService.Managers;

namespace AuthenticationService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<Settings>(Configuration);
            services.AddMvc(options => options.Filters.Add(typeof(GlobalExceptionFilter)));
            services
                //.AddTransient(_ => new DBManager(Configuration.GetConnectionString("DBConnection")))
                .AddTransient<IDBManager, DBManager>()
                .AddTransient<IAuthService, AuthService>();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"AuthenticationService. Current environment is {env.EnvironmentName}");
                });
            });
        }
    }
}
