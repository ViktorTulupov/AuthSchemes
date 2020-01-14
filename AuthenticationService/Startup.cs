using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationService.Filters;
using AuthenticationService.Services;
using Microsoft.Extensions.Hosting;
using AuthenticationService.Managers;
using Microsoft.OpenApi.Models;

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
                .AddTransient<IDBManager, DBManager>()
                .AddTransient<IAuthService, AuthService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AuthenticationService API",
                    Description = "API with ASP.NET Core 3.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Viktor Tulupov",
                        Email = "v.tulupov.personal@gmail.com",
                    },
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "AuthenticationService API V1");
                c.RoutePrefix = string.Empty;
                c.EnableValidator(null);
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync($"AuthenticationService. Current environment is {env.EnvironmentName}");
                //});
            });
        }
    }
}
