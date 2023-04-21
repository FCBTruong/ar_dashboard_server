using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.Admin;
using ar_dashboard.Models.Authentication;

namespace ar_dashboard.Services
{
    public interface IAdminDbService
    {
        Task<IEnumerable<AdminModel>> GetMultipleAsync(string query);
        Task<AdminModel> GetAsync();
        Task AddAsync(AdminModel item);
        Task UpdateAsync(AdminModel item);
        Task DeleteAsync();
    }
}
