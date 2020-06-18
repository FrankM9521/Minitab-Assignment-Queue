using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minitab.Assignment.CrmStub;
using Minitab.Assignment.CrmStub.Interfaces;
using Minitab.Assignment.Queues.Functions;
using Minitab.Assignment.Queues.Services.Models.Usps;
using Minitab.Assignment.Services;
using Minitab.Assignment.Services.Interfaces;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Minitab.Assignment.Queues.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<IAddressService, AddressService>()
                    .AddPolicyHandler(GetRetryPolicy());

            
            builder.Services.AddTransient<ICustomerService, CustomerService>();
            builder.Services.AddTransient<IAddressService, AddressService>();
            builder.Services.AddTransient<IAddressValidationService, UspsAddressValidationService>();
            builder.Services.AddTransient<ICrmRepository, CrmRepository>();
            builder.Services.AddTransient<ICustomerService, CustomerService>();
        }

        /// <summary>
        /// Retry Policy
        /// </summary>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
