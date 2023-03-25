using System;
using System.Threading.Tasks;
using ar_dashboard.Services;
using Microsoft.Extensions.Configuration;

namespace ar_dashboard.Controllers
{
    public class DatabaseController
    {
        public IUserDbService UserDbService { get; set; } // Db to manage user data
        public IAuthenDbService AuthenDbService { get; set; } // Db to manage authentication

        public DatabaseController(IConfigurationSection configurationSection)
        {
            UserDbService = InitializeUserDbInstanceAsync(configurationSection).GetAwaiter().GetResult();
            AuthenDbService = InitializeAuthenDbInstanceAsync(configurationSection).GetAwaiter().GetResult();
        }

        private static async Task<IUserDbService> InitializeUserDbInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["UserContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];

            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            var userDbService = new UserDbService(client, databaseName, containerName);
            return userDbService;
        }

        private static async Task<IAuthenDbService> InitializeAuthenDbInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["AuthenContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];

            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            var authenDbService = new AuthenDbService(client, databaseName, containerName);
            return authenDbService;
        }
    }
}
