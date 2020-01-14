using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AuthorizationService.Extensions;
using AuthorizationService.Filters;
using AuthorizationService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthorizationService
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
            services.AddAuthorization();
            services
                .AddAuthentication(Enums.AuthenticationSchemes.Basic)
                .AddBasicAuthentication<AuthenticationService>(options => { options.MethodName = Enums.AuthenticationSchemes.Basic; })
                .AddHMACAuthentication<AuthenticationService>(options => { options.MethodName = Enums.AuthenticationSchemes.HMAC; })
                .AddBeareruthentication<AuthenticationService>(options => { options.MethodName = Enums.AuthenticationSchemes.Bearer; });
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));
            services.AddHttpClient<IAuthenticationService, AuthenticationService>();
            services.Configure<Settings>(Configuration);
            services.AddMvc(options => options.Filters.Add(typeof(GlobalExceptionFilter)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"AuthorizationService. Current environment is {env.EnvironmentName}");
                });
            });
        }
    }
}
