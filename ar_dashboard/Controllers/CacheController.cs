using System;
using System.Threading.Tasks;
using ar_dashboard.Services;
using Microsoft.Extensions.Configuration;
using ar_dashboard.Models.Guest;
using Microsoft.Extensions.Caching.Memory;

namespace ar_dashboard.Controllers
{
    public class CacheController
    {
        private MuseumsInformation MuseumsInformation { get; set; }
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public CacheController()
        {
           
        }

        public MuseumsInformation GetMuseumsInformation()
        {
            return (MuseumsInformation)_cache.Get("museumsInformation");
        }

        public void SetMuseumsInformation(MuseumsInformation museumsInformation)
        {
            _cache.Set("museumsInformation", museumsInformation, new MemoryCacheEntryOptions()
          .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
          .SetPriority(CacheItemPriority.High));
        }
    }
}
