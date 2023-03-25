using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly IConfiguration _configuration;

        public UsersController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _configuration = configuration;
        }

        // GET api/users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var role = ushort.Parse(claim[2].Value);
            if(role != (ushort)UserRole.ADMIN)
            {
                return Unauthorized("Only admin has right to get list users");
            }
            return Ok(await _userDbService.GetMultipleAsync("SELECT * FROM c"));
        }

        // GET api/users/ user token to get userId
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var id = claim[0].Value;
            return Ok(await _userDbService.GetAsync(id));
        }

        // POST api/users
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserData item)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();

            item.Id = Guid.NewGuid().ToString();
            await _userDbService.AddAsync(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] UserData item)
        {
            //item.Id = 
            await _userDbService.UpdateAsync(item.Id, item);
            return NoContent();
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userDbService.DeleteAsync(id);
            return NoContent();
        }
    }
}
