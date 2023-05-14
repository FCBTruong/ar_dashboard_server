using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.ClientSendForm;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuseumsController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly CacheController _cacheController;

        public MuseumsController(DatabaseController databaseController, CacheController cacheController)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _cacheController = cacheController;
        }

        // GET api/museums/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // get from cache first
                var userData = _cacheController.GetUserData(userId);

                if (userData == null) // if data is not saved in cache, get from db
                {
                    userData = await _userDbService.GetAsync(userId);
                }

                if (userData == null)
                {
                    return BadRequest("user is null");
                }

                var museum = userData.Museums.Where((m) => m.Id == id);
                if(museum == null)
                {
                    return BadRequest("museum is null");
                }
                return Ok(museum);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        // POST api/museums/
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMuseumForm museumForm)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;


                if(museumForm == null || museumForm.Name.Length == 0)
                {
                    return BadRequest("error with parameters");
                }
                var museum = new Museum();
                museum.Name = museumForm.Name;
                museum.Introduction = museumForm.Introduction;
                museum.Address = museumForm.Address;
                museum.ImageUrl = museumForm.ImageUrl;

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

                userData.Museums.Add(museum);

                // save to db
                await _userDbService.UpdateAsync(userId, userData);
                _cacheController.SetUserData(userId, userData); // save to cache
                return Ok(museum);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        // PUT api/museums/5
        [HttpPut("{museumId}")]
        [Authorize]
        public async Task<IActionResult> Update(string museumId, [FromBody] Museum museumUpdate)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var userData = await _userDbService.GetAsync(userId);

                if (userData == null)
                {
                    return BadRequest("user is null");
                }

                var museum = userData.Museums.Find((m) => m.Id == museumId);
                if (museum == null)
                {
                    return BadRequest("museum is null");
                }

                museum.Name = museumUpdate.Name;
                museum.Introduction = museumUpdate.Introduction;
                museum.ImageUrl = museumUpdate.ImageUrl;
                museum.Address = museumUpdate.Address;
                museum.OpeningTime = museumUpdate.OpeningTime;

                await _userDbService.UpdateAsync(userId, userData);
                _cacheController.SetUserData(userId, userData); // save to cache
                return Ok(museum);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        // DELETE api/museums/
        [HttpDelete("{museumId}")]
        [Authorize]
        public async Task<IActionResult> Delete(string museumId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;


                if (museumId == "null" || museumId.Length < 1)
                {
                    return BadRequest("error with parameters");
                }

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

                int idxMuseum = userData.Museums.FindIndex(m => m.Id == museumId);
                if(idxMuseum == -1)
                {
                    return BadRequest("No museum to delete");
                }

                userData.Museums.RemoveAt(idxMuseum);

                // save to db
                await _userDbService.UpdateAsync(userId, userData);
                _cacheController.SetUserData(userId, userData); // save to cache
                return Ok(userData);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
