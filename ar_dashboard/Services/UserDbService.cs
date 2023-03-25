using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.Azure.Cosmos;

namespace ar_dashboard.Services
{
    public class UserDbService : IUserDbService
    {
        private Container _container;

        public UserDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(UserData item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<UserData>(id, new PartitionKey(id));
        }

        public async Task<UserData> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<UserData>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<UserData>> GetMultipleAsync(string queryString)
        {
            Console.WriteLine("UserDb query: " + queryString);
            var query = _container.GetItemQueryIterator<UserData>(new QueryDefinition(queryString));

            var results = new List<UserData>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(string id, UserData item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
