using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.Data;

namespace ar_dashboard.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<ObjectData>> GetMultipleAsync(string query);
        Task<ObjectData> GetAsync(string id);
        Task AddAsync(ObjectData user);
        Task UpdateAsync(string id, ObjectData item);
        Task DeleteAsync(string id);
    }
}
