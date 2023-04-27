using System;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.Guest;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IAdminDbService _adminDbService;
        private MuseumsInformation _museumsInformation;
        private readonly IUserDbService _userDbService;
        private readonly CacheController _cacheController;
        public PublicController(DatabaseController databaseController, CacheController cacheController)
        {
            _adminDbService = databaseController.AdminDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _cacheController = cacheController ?? throw new ArgumentNullException(nameof(cacheController));
        }

        [HttpPost]
        public async Task<IActionResult> RequestPublish()
        {
            try
            {
                var adminModel = await _adminDbService.GetAsync();


                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpPut]
        [Route("refresh")]
        public string RefreshPublicMuseums()
        {
            try
            {
                _museumsInformation = null;
                return "ok";
            }
            catch (Exception e)
            {
                return $"Internel server error: {e}";
            }
        }

        [HttpGet]
        [Route("museums")]
        public async Task<IActionResult> GetPublicMuseums()
        {
            try
            {
                _museumsInformation = _cacheController.GetMuseumsInformation();
                if (_museumsInformation != null)
                {
                    return Ok(_museumsInformation);
                }
                else
                {
                    // get data
                    //var adminModel = await _adminDbService.GetAsync();
                    // TODO later
                    var users = (await _userDbService.GetMultipleAsync($"SELECT * FROM c")).ToArray();

                    _museumsInformation = new MuseumsInformation();

                    // tam thoi lay het
                    foreach (UserData userData in users)
                    {
                        foreach (Museum museum in userData.Museums)
                        {
                            PublicMuseum publicMuseum = new PublicMuseum();
                            publicMuseum.Name = museum.Name;
                            publicMuseum.Id = museum.Id;
                            publicMuseum.Address = museum.Address;
                            publicMuseum.OpeningTime = museum.OpeningTime;
                            publicMuseum.ImageUrl = museum.ImageUrl;
                            _museumsInformation.Museums.Add(publicMuseum);


                        }
                    }
                    _cacheController.SetMuseumsInformation(_museumsInformation);
                    return Ok(_museumsInformation);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
