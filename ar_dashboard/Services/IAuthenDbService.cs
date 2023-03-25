using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.Authentication;

namespace ar_dashboard.Services
{
    public interface IAuthenDbService
    {
        Task<IEnumerable<AuthenModel>> GetMultipleAsync(string query);
        Task<AuthenModel> GetAsync(string id);
        Task AddAsync(AuthenModel item);
        Task UpdateAsync(string id, AuthenModel item);
        Task DeleteAsync(string id);
    }
}
