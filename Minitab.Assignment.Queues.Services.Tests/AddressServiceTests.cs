using Minitab.Assignment.DomainModels;
using Minitab.Assignment.Queues.Services.Models.Usps;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Minitab.Assignment.Services.Tests
{
    public class MyHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
    
    public class AddressServiceTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IUspsSettings _options;
        public AddressServiceTests()
        {
            _httpClientFactory = new MyHttpClientFactory();
            _options = new UspsSettings
            {
                Url = "http://production.shippingapis.com/ShippingAPI.dll",
                UserName = "633FMCON0605"
            });
        }

        [Fact]
        public async Task WhenAddressIsValid_ThenResult_ShouldBeTrue()
        {
            var svc = new AddressService(new UspsAddressValidationService(), _httpClientFactory);
            var addressDomainModel = new AddressDomainModel
            {
                City = "Chicago",
                State = "IL",
                PostalCode = "60606",
                Line1 = "233 S. Wacker Dr."
            };

            var result = await svc.ValidAddressAsync(addressDomainModel);
            Assert.True(result);
        }

        [Fact]
        public async Task WhenAddressInIsValid_ThenResult_ShouldBeFalse()
        {
            var svc = new AddressService(new UspsAddressValidationService(), _httpClientFactory);
            var addressDomainModel = new AddressDomainModel
            {
                City = "Chicago",
                State = "NE",
                PostalCode = "10101",
                Line1 = "233 S. Wacker Dr."
            };

            var result = await svc.ValidAddressAsync(addressDomainModel);
            Assert.False(result);
        }
    }
}
