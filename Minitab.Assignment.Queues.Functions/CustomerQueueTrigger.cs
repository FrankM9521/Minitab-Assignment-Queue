using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Minitab.Assignment.DomainModels;
using Minitab.Assignment.Services.Interfaces;
using Newtonsoft.Json;

namespace Minitab.Assignment.Queues.Functions
{
    public  class CustomerQueueTrigger
    {
        private readonly ICustomerService _customerService;
        public CustomerQueueTrigger(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [FunctionName("QueueCustomer")]
        public  async Task Run([QueueTrigger("customer-items", Connection = "AzureWebJobsStorage")]CustomerDomainModel customerQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed sonething");

            await _customerService.UpsertCustomerAsync(customerQueueItem);
        }
    }
}
