using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ar_dashboard.Models;

namespace ar_dashboard.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<UserData>> GetMultipleAsync(string query);
        Task<UserData> GetAsync(string id);
        Task AddAsync(UserData user);
        Task UpdateAsync(string id, UserData item);
        Task DeleteAsync(string id);
    }
}
