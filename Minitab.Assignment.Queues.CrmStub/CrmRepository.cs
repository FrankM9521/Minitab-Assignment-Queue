using Minitab.Assignment.CrmStub.Interfaces;
using Minitab.Assignment.DomainModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minitab.Assignment.CrmStub
{
    [Serializable]
    public class CustomerList
    {
        public List<CustomerDomainModel> Customers { get; set; } = new List<CustomerDomainModel>();
    }
    /// <summary>
    /// File System Repository
    /// </summary>
    public class CrmRepository : ICrmRepository
    {
        private readonly string _fileName = "Customers.txt";
        private readonly string _pathAndFile;
        public CrmRepository()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _pathAndFile = Path.Combine(path, _fileName);
        }

        /// <summary>
        /// Fake CRM Stub. Inserts customer
        /// </summary>
        /// <param name="customerDomainModel"></param>
        /// <returns></returns>
        public async Task UpsertCustomer(CustomerDomainModel customerDomainModel)
        {
            var list = await ReadFromFileAsync<CustomerList>(_pathAndFile);

            if (list == null)
            {
                list = new CustomerList();
            }
            list.Customers.Add(customerDomainModel);
            await WriteToFileAsync<CustomerList>(_pathAndFile, list);
        }

        /// <summary>
        /// Just to help with end to end testing
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public  async Task<CustomerDomainModel> GetByEmail(string emailAddress)
        {
            var list = await ReadFromFileAsync<CustomerList>(_pathAndFile);
            return list?.Customers.Where(c => c.CustomerEmail.Equals(emailAddress)).FirstOrDefault();
        }

        /// <summary>
        /// Just to help with end to end testing
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await WriteToFileAsync<string>(_pathAndFile, "");
        }

        static async Task<T> ReadFromFileAsync<T>(string filePath)
        {
            var bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];    
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read, bufferSize: bufferSize, useAsync: true))
            {
                var readBuffer = new StringBuilder();
                var bytesRead = 0;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    readBuffer.Append(Encoding.Unicode.GetString(buffer, 0, bytesRead));
                }

                return JsonConvert.DeserializeObject<T>(readBuffer.ToString());
            }
        }
        static async Task WriteToFileAsync<T>(string filePath, T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            byte[] buffer = Encoding.Unicode.GetBytes(json);
            var offset = 0;
            var sizeOfBuffer = 4096;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: sizeOfBuffer, useAsync: true))
                await fileStream.WriteAsync(buffer, offset, buffer.Length);
        }
    }
}
