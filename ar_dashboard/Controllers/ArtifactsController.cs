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

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtifactsController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly IConfiguration _configuration;

        public ArtifactsController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateArtifact([FromBody] CreateArtifactForm artifactForm)
        {
            try
            {
                Artifact artifact = artifactForm.Artifact;
                artifact.Id = Guid.NewGuid().ToString();

                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                // Find museum with id
                var museumId = artifactForm.MuseumId;
                var museum = userData.Museums.Find(museum => museum.Id == museumId);
                if (museum == null)
                {
                    return BadRequest("Can not find museum");
                }

                museum.Artifacts.Add(artifact);

                await _userDbService.UpdateAsync(userId, userData);

                return Ok(artifact);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateArtifact([FromBody] CreateArtifactForm artifactForm)
        {
            try
            {
                Artifact artifact = artifactForm.Artifact;

                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                // Find museum with id
                var museumId = artifactForm.MuseumId;
                var museum = userData.Museums.Find(museum => museum.Id == museumId);
                if (museum == null)
                {
                    return BadRequest("Can not find museum");
                }

                // Find artifact by id
                int check = museum.Artifacts.FindIndex(_artifact => _artifact.Id == artifact.Id);
                if (check == -1)
                {
                    return BadRequest("Artifact not found");
                }
                museum.Artifacts[check] = artifact;

                await _userDbService.UpdateAsync(userId, userData);

                return Ok(artifact);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteArtifact(string museumId, string artifactId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var userData = await _userDbService.GetAsync(userId);
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
                museum.Artifacts.RemoveAt(index);

                await _userDbService.UpdateAsync(userId, userData);

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
        [Authorize]
        [HttpDelete("{museumId")]
        public async Task<IActionResult> DeleteAll(string museumId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                var museum = userData.Museums.Find(museum => museum.Id == museumId);
                if (museum == null)
                {
                    return BadRequest("Can not find museum");
                }


                museum.Artifacts.Clear();

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

