using AuthorizationService.Extensions;
using AuthorizationService.Filters;
using AuthorizationService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            services.AddHttpClient<IAuthenticationService, AuthenticationService>();
            services.Configure<Settings>(Configuration);
            services.AddMvc(options => options.Filters.Add(typeof(GlobalExceptionFilter)));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AuthorizationService API",
                    Description = "API with ASP.NET Core 3.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Viktor Tulupov",
                        Email = "v.tulupov.personal@gmail.com",
                    },
                });
                c.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "basic"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "basicAuth" }
                        },
                        new string[]{}
                    }
                });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                        },
                        new string[]{}
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "AuthorizationService API V1");
                c.RoutePrefix = string.Empty;
                c.EnableValidator(null);
            });
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
