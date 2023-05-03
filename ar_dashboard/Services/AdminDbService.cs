using System;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using ar_dashboard.Models.Admin;

namespace ar_dashboard.Services
{
    public class AdminDbService : IAdminDbService
    {
        private Container _container;

        public AdminDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(AdminModel item)
        {
            await _container.CreateItemAsync(item, new PartitionKey("admin"));
        }

        public async Task DeleteAsync()
        {
            await _container.DeleteItemAsync<AdminModel>("admin", new PartitionKey("admin"));
        }

        public async Task<AdminModel> GetAsync()
        {
            try
            { 
                var response = await _container.ReadItemAsync<AdminModel>("admin", new PartitionKey("admin"));
                return response.Resource;
            }
            catch (CosmosException e) //For handling item not found and other exceptions
            {
                if(e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var adminModel = new AdminModel();
                    await AddAsync(adminModel);
                    return adminModel;
                }
                return null;
            }
        }

        public async Task<IEnumerable<AdminModel>> GetMultipleAsync(string queryString)
        {
            Console.WriteLine("Query----" + queryString);
            var query = _container.GetItemQueryIterator<AdminModel>(new QueryDefinition(queryString));

            var results = new List<AdminModel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(AdminModel item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey("admin"));
        }
    }
}
