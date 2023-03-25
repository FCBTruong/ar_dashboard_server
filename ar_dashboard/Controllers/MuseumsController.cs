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

        public MuseumsController(DatabaseController databaseController)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
        }

        // GET api/museums/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var userData =  await _userDbService.GetAsync(id);

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

                var userData = await _userDbService.GetAsync(userId);

                if(userData == null)
                {
                    return BadRequest("user data is null");
                }

                userData.Museums.Add(museum);

                // save to db
                await _userDbService.UpdateAsync(userId, userData);
                return Ok();

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
