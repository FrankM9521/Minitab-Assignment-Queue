using System;

namespace Minitab.Assignment.Queues.Services.Models.Usps
{
    public interface IUspsSettings
    {
        string UserName { get; set; }
        string Url { get; }
    }
    /// <summary>
    /// Settings for USPS
    /// </summary>
    public class UspsSettings : IUspsSettings
    {
        private string _userName;
        private string _url;
        
        public UspsSettings()
        {
            _userName =  Environment.GetEnvironmentVariable("UspsUserName");
            _url =  Environment.GetEnvironmentVariable("UspsUrl");
        }
        public string UserName
        {
            get  { return _userName; }
            set { _userName = value; }
        }
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
    }
}
