using Microsoft.Extensions.Options;
using Minitab.Assignment.Common.Extensions;
using Minitab.Assignment.Common.Models;
using Minitab.Assignment.DomainModels;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Minitab.Assignment.Queues.QueueClient
{
    public interface ICustomerQueueClient
    {
        Task EnqueueCustomer(CustomerDomainModel customer);
    }
    public class CustomerQueueClient : ICustomerQueueClient
    {
        private readonly IOptions<CustomerQueueSettings> _options;
        public CustomerQueueClient(IOptions<CustomerQueueSettings> options)
        {
            _options = options;
        }

        public async Task EnqueueCustomer(CustomerDomainModel customer)
        {
            var customerJson = JsonConvert.SerializeObject(customer);
            var queueClient = new Azure.Storage.Queues.QueueClient(_options.Value.AzureWebJobsStorage, _options.Value.QueueName);

            if (queueClient.Exists())
            {
                // Send a message to the queue
                await  queueClient.SendMessageAsync(customerJson.ToBase64());
            }
        }
    }
}
