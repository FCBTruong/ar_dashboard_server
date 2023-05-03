using System;
using System.Threading.Tasks;
using ar_dashboard.Services;
using Microsoft.Extensions.Configuration;
using ar_dashboard.Models.Guest;
using Microsoft.Extensions.Caching.Memory;
using ar_dashboard.Models.Admin;

namespace ar_dashboard.Controllers
{
    public class CacheController
    {
        private MuseumsInformation MuseumsInformation { get; set; }
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
    }
}
