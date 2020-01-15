using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                c.SwaggerDoc("basic", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Basic authentication api",
                    Description = "API with ASP.NET Core 3.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Viktor Tulupov",
                        Email = "v.tulupov.personal@gmail.com",
                    },
                });
                c.SwaggerDoc("hmac", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "HMAC authentication api",
                    Description = "API with ASP.NET Core 3.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Viktor Tulupov",
                        Email = "v.tulupov.personal@gmail.com",
                    },
                });
                c.SwaggerDoc("bearer", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT authentication api",
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
            app.UseSwagger(c => c.RouteTemplate = "{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/basic/swagger.json", "Basic authentication");
                c.SwaggerEndpoint("/hmac/swagger.json", "HMAC authentication");
                c.SwaggerEndpoint("/bearer/swagger.json", "JWT authentication");
                c.RoutePrefix = string.Empty;
                c.EnableValidator(null);
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
