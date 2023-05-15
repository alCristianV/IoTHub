using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IoTHubAPI.DatabaseSettings;
using Microsoft.Extensions.Options;
using IoTHubAPI.Repositories;
using IoTHubAPI.Controllers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using IoTHubAPI.Helpers;
using IoTHubAPI.SignalR;

namespace IoTHubAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<IotHubDatabaseSettings>(
                Configuration.GetSection(nameof(IotHubDatabaseSettings)));

            services.AddSingleton<IIotHubDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<IotHubDatabaseSettings>>().Value);
            services.AddSingleton<PresenceTracker>();
            services.TryAddScoped<IAuthRepository, AuthRepository>();
            services.TryAddScoped<IUserRepository, UserRepository>();
            services.TryAddScoped<IDeviceRepository, DeviceRepository>();
            services.TryAddScoped<INotificationRepository, NotificationRepository>();
            services.TryAddScoped<IMessageRepository, MessageRepository>();
            services.TryAddScoped<IDeviceDataFieldRepository, DeviceDataFieldRepository>();
            services.TryAddScoped<IActionRepository, ActionRepository>();
            services.AddAutoMapper(typeof(UserRepository).Assembly);
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("IotHubAPISpec",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "IotHubAPI",
                        Version = "1"
                    });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                        .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddCors();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler(
                    builder => {
                        builder.Run(async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null) {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message);
                            }
                        });
                    });
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("swagger/IotHubAPISpec/swagger.json", "IotHubAPI");
                options.RoutePrefix = "";
            });
            app.UseRouting();

            app.UseCors(x => x.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/message");
            });

            
        }
    }
}
