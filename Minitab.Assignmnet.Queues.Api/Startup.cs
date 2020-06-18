using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Minitab.Assignment.Common.Models;
using Minitab.Assignment.CrmStub;
using Minitab.Assignment.CrmStub.Interfaces;
using Minitab.Assignment.Queues.Api.Middleware;
using Minitab.Assignment.Queues.Api.Models;
using Minitab.Assignment.Queues.QueueClient;
using Minitab.Assignment.Services;
using Minitab.Assignment.Services.Interfaces;

namespace Minitab.Assignmnet.Queues.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Swagger 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Minitab Address Validation",
                    Version = "v1",
                    Description = "Sample API By Frank Malinowski"
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<ICustomerQueueClient, CustomerQueueClient>();
            services.AddHttpClient<IAddressService, AddressService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<IAddressValidationService, UspsAddressValidationService>();
            services.AddTransient<ICrmRepository, CrmRepository>();

            services.Configure<CustomerQueueSettings>(Configuration.GetSection("CustomerQueueSettings"));
            services.AddOptions();

            services
               .AddControllers()
                   .ConfigureApiBehaviorOptions(options =>
                   {
                       options.InvalidModelStateResponseFactory = context =>
                       {
                           var error = new ErrorMessages
                           {
                               Message = "The request is invalid.",
                               Errors = context.ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)).ToList()
                           };

                           var result = new BadRequestObjectResult(error);
                           result.ContentTypes.Add(MediaTypeNames.Application.Json);
                           result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                           return result;
                       };
                   })
                    .AddNewtonsoftJson();


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            //Global error handling
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minitab");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
