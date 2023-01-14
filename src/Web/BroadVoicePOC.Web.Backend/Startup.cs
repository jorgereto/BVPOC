// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BroadVoicePOC.Business.Interfaces;
using BroadVoicePOC.Business.Services;
using BroadVoicePOC.Common.Constants.Enums;
using BroadVoicePOC.DataAccess.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using AutoMapper;

namespace BroadVoicePOC.Web.Backend
{
    public class Startup
    {
        public bool IsDebug(IWebHostEnvironment env)
        {
            return env.IsDevelopment() || env.IsEnvironment("Local") || env.IsEnvironment("Debug");
        }

        public bool IsLocal(IWebHostEnvironment env)
        {
            return env.IsDevelopment() || env.IsEnvironment("Local");
        }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            CurrentEnvironment = env;
            _httpLogging = Enum.Parse<HTTPLogging>(Configuration["HTTPLogging"]);
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        private HTTPLogging _httpLogging { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(ConfigureMvcOptions)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            // db context
            services.AddDbContext<BroadVoicePOCContext>(options =>
            {
                // Configure Entity Framework Core to use Microsoft SQL Server.
                options.UseSqlServer(Configuration.GetConnectionString("BroadVoicePOC"));
            });

            // http context available for all controllers
            services.AddHttpContextAccessor();

            // swagger generator
            if (bool.Parse(Configuration["EnableSwagger"]))
                services.AddSwaggerGen();

            // automapper
            services.AddAutoMapper(typeof(Startup));

            // services
            services.AddScoped<ISalesService, SalesService>();
            services.AddScoped<ISalespersonService, SalespersonService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAppService, AppService>();

            // file logging
            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddFile(CurrentEnvironment.ContentRootPath + Configuration["Logging:File:Path"], fileLoggerOpts =>
                {
                    fileLoggerOpts.Append = bool.Parse(Configuration["Logging:File:Append"]);
                    fileLoggerOpts.MinLevel = Enum.Parse<LogLevel>(Configuration["Logging:File:MinLevel"]);
                    fileLoggerOpts.FileSizeLimitBytes = int.Parse(Configuration["Logging:File:FileSizeLimitBytes"]);
                    fileLoggerOpts.MaxRollingFiles = int.Parse(Configuration["Logging:File:MaxRollingFiles"]);
                    fileLoggerOpts.FormatLogFileName = fName => { return String.Format(fName, DateTime.Now); };
                    fileLoggerOpts.FormatLogEntry = (msg) =>
                    {
                        return String.Format("{0} [{1}] {2} {3} {4} {5}",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"),
                            msg.EventId.Id,
                            msg.LogLevel.ToString(),
                            msg.LogName,
                            msg.Message,
                            msg.Exception?.ToString());
                    };
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (IsDebug(env))
                app.UseDeveloperExceptionPage();

            if (!IsLocal(env))
                app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            // swagger
            if (bool.Parse(Configuration["EnableSwagger"]))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        private void ConfigureMvcOptions(MvcOptions mvcOptions)
        {
        }
    }
}
