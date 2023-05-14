using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models.Guest;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    public class GuestsController : Controller
    {
        private readonly IUserDbService _userDbService;
        private readonly IAdminDbService _adminDbService;
        private readonly String connectionString;
        private readonly CacheController _cacheController;
        public GuestsController(DatabaseController databaseController, IConfiguration configuration, CacheController cacheController)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _adminDbService = databaseController.AdminDbService ?? throw new ArgumentNullException(nameof(databaseController));
            connectionString = configuration["AzureFiles:ConnectionString"];
            _cacheController = cacheController;
        }


        [HttpGet("{userId}/{museumId}/{artifactId}")]
        public async Task<IActionResult> GetArtifactInfo(string userId, string museumId, string artifactId)
        {
            try
            {
                if (userId == "") return BadRequest("Id is empty");
                // get from cache first
                var userData = _cacheController.GetUserData(userId);

                if (userData == null) // if data is not saved in cache, get from db
                {
                    userData = await _userDbService.GetAsync(userId);
                }
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                var museum = userData.Museums.Find(museum => museum.Id == museumId);
                if (museum == null)
                {
                    return BadRequest("Can not find museum");
                }

                // Find artifact by id
                int index = museum.Artifacts.FindIndex(_artifact => _artifact.Id == artifactId);
                if (index == -1)
                {
                    return BadRequest("Artifact not found");
                }

                var artifact = museum.Artifacts[index];
                var artifactPackage = new ArtifactInformation();
                artifactPackage.Artifact = artifact;
                artifactPackage.MuseumId = museumId;
                artifactPackage.Id = artifact.Id;
                artifactPackage.MuseumName = museum.Name;

                return Ok(artifactPackage);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
