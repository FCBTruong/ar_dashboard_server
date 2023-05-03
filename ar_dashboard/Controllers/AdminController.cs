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
    public class AdminController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly IAuthenDbService _authenDbService;
        private readonly IAdminDbService _adminDbService;

        private readonly IConfiguration _configuration;
        private readonly CacheController _cacheController;

        public AdminController(DatabaseController databaseController, IConfiguration configuration, CacheController cacheController)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _authenDbService = databaseController.AuthenDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _adminDbService = databaseController.AdminDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _cacheController = cacheController;
            _configuration = configuration;
        }

        // DELETE api/admin
        [HttpDelete("delete-user-data")]
        [Authorize]
        public async Task<IActionResult> DeleteUserData(string id)
        {
            try
            {
                if (id == null) return BadRequest("id is invalid");
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admin has right to get list users");
                }
                await _userDbService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        // DELETE api/admin/delete-authen/
        [HttpDelete("delete-authen")]
        [Authorize]
        public async Task<IActionResult> DeleteAuthen(string id)
        {
            try
            {
                if (id == null) return BadRequest("id is invalid");
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admin has right to get list users");
                }
                await _authenDbService.DeleteAsync(id);
                await _userDbService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get-data")]
        public async Task<IActionResult> GetAdminData()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admins");
                }

                var adminModel = _cacheController.GetAdminModel();
                if (adminModel == null)
                {
                    // get data
                    adminModel = await _adminDbService.GetAsync();
                    _cacheController.SetAdminData(adminModel);
                }

                if (adminModel == null)
                {
                    return StatusCode(500, "admin model is null");
                }

                return Ok(adminModel);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
