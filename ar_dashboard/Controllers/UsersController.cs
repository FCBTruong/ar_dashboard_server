﻿using System;
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

        // GET api/users/get-list
        [HttpGet("get-list")]
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
        [Authorize]
        [HttpGet("get-info")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var id = claim[0].Value;

                var user = await GetUserData(id);
                return Ok(user);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        private async Task<UserData> GetUserData(string id)
        {
            // get from database
            var user = await _userDbService.GetAsync(id);
            if(user == null)
            {
                // user not created in db yet -> create new
                user = new UserData();
                user.Id = id;
                await _userDbService.AddAsync(user);
            }
            return user;
        }


        // PUT api/users/edit
        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> Edit([FromBody] UserData item)
        {
            try
            {
                await _userDbService.UpdateAsync(item.Id, item);
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
