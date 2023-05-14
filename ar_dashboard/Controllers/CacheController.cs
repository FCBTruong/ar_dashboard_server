using System;
using System.Threading.Tasks;
using ar_dashboard.Services;
using Microsoft.Extensions.Configuration;
using ar_dashboard.Models.Guest;
using Microsoft.Extensions.Caching.Memory;
using ar_dashboard.Models.Admin;
using ar_dashboard.Models;

namespace ar_dashboard.Controllers
{
    public class CacheController
    {
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public CacheController()
        {

        }

        public AdminModel GetAdminModel()
        {
            return (AdminModel)_cache.Get("adminModel");
        }

        public void SetAdminData(AdminModel adminModel)
        {
            _cache.Set("adminModel", adminModel, new MemoryCacheEntryOptions()
         .SetAbsoluteExpiration(TimeSpan.FromMinutes(1000))
         .SetPriority(CacheItemPriority.High));
        }

        public void SetUserData(string uId, UserData userData)
        {
            _cache.Set(uId, userData, new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30))
        .SetPriority(CacheItemPriority.High));
        }

        public UserData GetUserData(string uId)
        {
            return (UserData)_cache.Get(uId);
        }
    }
}
