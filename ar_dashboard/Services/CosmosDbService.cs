using System;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using ar_dashboard.Models.Data;

namespace ar_dashboard.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddAsync(ObjectData item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
        }

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<ObjectData>(id, new PartitionKey(id));
        }

        public async Task<ObjectData> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<ObjectData>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException) //For handling item not found and other exceptions
            {
                return null;
            }
        }

        public async Task<IEnumerable<ObjectData>> GetMultipleAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<ObjectData>(new QueryDefinition(queryString));

            var results = new List<ObjectData>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateAsync(string id, ObjectData item)
        {
            await _container.UpsertItemAsync(item, new PartitionKey(id));
        }
    }
}
