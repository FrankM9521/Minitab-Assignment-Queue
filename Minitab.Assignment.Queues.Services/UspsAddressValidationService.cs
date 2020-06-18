using Minitab.Assignment.Common.Utility;
using Minitab.Assignment.CrmStub.Interfaces;
using Minitab.Assignment.DomainModels;
using Minitab.Assignment.Queues.Services.Models.Usps;
using Minitab.Assignment.Services.Interfaces;
using Minitab.Assignment.Services.Mappers;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Minitab.Assignment.Services
{
    /// <summary>
    /// Validates an address using the USPS web service
    /// All USPS logic and references are internal to this implementation
    /// to make it easier to change validation services
    /// </summary>
    public class UspsAddressValidationService : IAddressValidationService
    {
        private readonly IUspsSettings _options;
        public UspsAddressValidationService()
        {
            //had DI issues here, this is the 2nd implementaton I've done, I hate to say it but I'm just too exhausted to figure out the issue and 
            //need to ship this
            _options = new UspsSettings();
        }
        public string CreateRequest(AddressDomainModel addressDomainModel)
        {
            var addressXml = GetAddressAsXml(addressDomainModel);
            return BuildAddressValidationUrl(addressXml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool IsValidAddress(Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            return !doc.Root.Elements().Descendants("Error").Any();
        }

        /// <summary>
        /// Converts an address domain model to an XML string
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private string GetAddressAsXml(AddressDomainModel addressDomainModel)
        {
            var addressValidateRequest = addressDomainModel.ToAddressValidateRequest(_options.UserName);

            return XmlSerializationUtility.GetXmlString(addressValidateRequest);
        }

        /// <summary>
        /// Creates the full Url with parameters
        /// </summary>
        /// <param name="addressXml"></param>
        /// <returns></returns>
        private string BuildAddressValidationUrl(string addressXml)
        {
            return $"{_options.Url}?API=verify&XML={addressXml}";
        }
    }
}
