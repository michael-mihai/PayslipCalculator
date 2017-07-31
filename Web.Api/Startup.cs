using Domain.Models.Configuration;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Swagger;
using Web.Api.Extensions;
using WebApiContrib.Core.Formatter.Csv;

namespace Web.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var csvOptions = new CsvFormatterOptions
            {
                UseSingleLineHeaderInCsv = true,
                CsvDelimiter = ","
            };

            // Add framework services.
            // Set input and output formatters for csv request and response
            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new CsvInputFormatter(csvOptions));
                options.OutputFormatters.Add(new CsvOutputFormatter(csvOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Payslip Calculator",
                    Version = "v1",
                    TermsOfService = "None",
                    Description = "A simple API to calculate payslips based on configured tax brackets" 
                });
            });

            // dependency injection setup; TaxSettings are loaded from application.json
            services.AddSingleton<IPayslipService, PayslipService>();
            services.ConfigurePOCO<TaxSettings>(Configuration.GetSection("TaxSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // DI for logging
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payslip Calculator API V1");
            });
        }
    }
}
