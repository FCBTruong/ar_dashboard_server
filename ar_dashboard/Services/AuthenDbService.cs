using System;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using ar_dashboard.Models.Authentication;

namespace ar_dashboard.Services
{
    public class AuthenDbService : IAuthenDbService
    {
        private Container _container;

        public AuthenDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(AuthenModel item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<AuthenModel>(id, new PartitionKey(id));
        }

        public async Task<AuthenModel> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<AuthenModel>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<AuthenModel>> GetMultipleAsync(string queryString)
        {
            Console.WriteLine("Query----" + queryString);
            var query = _container.GetItemQueryIterator<AuthenModel>(new QueryDefinition(queryString));

            var results = new List<AuthenModel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(string id, AuthenModel item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
