using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Models.Admin;
using ar_dashboard.Models.Guest;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut]
        [Authorize]
        [Route("refresh")]
        public string RefreshPublicMuseums()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return ("Only admin has right to get list users");
                }

                _museumsInformation = null;
                return "ok";
            }
            catch (Exception e)
            {
                return $"Internel server error: {e}";
            }
        }

        [HttpPut]
        [Authorize]
        [Route("request-publish-museum")]
        public async Task<IActionResult> RequestPublicMuseums(Museum museum)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                var userId = claim[0].Value;

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

                var museumPost = new PublicMuseumPost();
                museumPost.UserId = userId;
                museumPost.Museum = museum;

                // remove old post
                var idx = adminModel.PendingMuseums.FindIndex(museum => museum.UserId == museumPost.UserId);
                if(idx != -1)
                {
                    adminModel.PendingMuseums.RemoveAt(idx);
                }

                adminModel.PendingMuseums.Add(museumPost);

                _cacheController.SetAdminData(adminModel); // save to ram
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpGet]
        [Route("museums")]
        public async Task<IActionResult> GetPublicMuseums()
        {
            try
            {
                var adminModel = _cacheController.GetAdminModel();
                if (adminModel == null)
                {
                    // get data
                    adminModel = await _adminDbService.GetAsync();
                    _cacheController.SetAdminData(adminModel);
                }

                if(adminModel == null)
                {
                    return StatusCode(500, "admin model is null");
                }

                var publishedMuseums = adminModel.PublicizedMuseums;

                _museumsInformation = new MuseumsInformation();


                foreach (PublicMuseumPost post in publishedMuseums)
                {
                    var museum = post.Museum;
                    PublicMuseum publicMuseum = new PublicMuseum();
                    publicMuseum.Name = museum.Name;
                    publicMuseum.Id = museum.Id;
                    publicMuseum.Address = museum.Address;
                    publicMuseum.OpeningTime = museum.OpeningTime;
                    publicMuseum.ImageUrl = museum.ImageUrl;
                    _museumsInformation.Museums.Add(publicMuseum);
                }


                return Ok(_museumsInformation);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("pending-museums")]
        public async Task<IActionResult> GetPendingMuseums()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admin has right to get list users");
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

                var pendingMuseums = adminModel.PendingMuseums;

                _museumsInformation = new MuseumsInformation();

                return Ok(pendingMuseums);

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpPut]
        [Authorize]
        [Route("accept-pending")]
        public async Task<IActionResult> AcceptPendingMuseum(string userId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admin has right to get list users");
                }

                var adminModel = _cacheController.GetAdminModel();
                if (adminModel == null)
                {
                    return BadRequest();
                }

                var idx = adminModel.PendingMuseums.FindIndex(museum => museum.UserId == userId);

                if(idx != -1)
                {
                    var pendingMuseum = adminModel.PendingMuseums[idx];
                    adminModel.PendingMuseums.RemoveAt(idx);

                    // tried to find if publiscied
                    var oldPublishedIdx = adminModel.PublicizedMuseums.FindIndex(museum => museum.UserId == userId);
                    adminModel.PublicizedMuseums.RemoveAt(oldPublishedIdx);

                    adminModel.PublicizedMuseums.Add(pendingMuseum);

                    _cacheController.SetAdminData(adminModel);
                    await _adminDbService.UpdateAsync(adminModel);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpPut]
        [Authorize]
        [Route("reject-pending")]
        public IActionResult RejectPendingMuseum(string userId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var role = ushort.Parse(claim[2].Value);
                if (role != (ushort)UserRole.ADMIN)
                {
                    return Unauthorized("Only admin has right to get list users");
                }

                var adminModel = _cacheController.GetAdminModel();
                if (adminModel == null)
                {
                    return BadRequest("not init yet, should load admin model first");
                }

                var idx = adminModel.PendingMuseums.FindIndex(museum => museum.UserId == userId);

                if (idx != -1)
                {
                    var pendingMuseum = adminModel.PendingMuseums[idx];
                    adminModel.PendingMuseums.RemoveAt(idx);
                    
                    _cacheController.SetAdminData(adminModel);
                    return Ok();
                }
                return BadRequest("not found");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
