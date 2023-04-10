    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Asset3DController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        public Asset3DController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));

        }

        [Authorize]
        [HttpPost]
        [RequestSizeLimit(737280000)]
        public async Task<IActionResult> CreateAsset3D(IFormFile file)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage
                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                var folderName = Path.Combine("Resources/Files", userId);
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Console.WriteLine("path to save" + pathToSave);

                if (file.Length > 0)
                {
                    var assetId = Guid.NewGuid().ToString();
                    var fileName = assetId + "";
                    string extension = Path.GetExtension(file.FileName);

                    if(extension != ".glb" && extension != ".fbx")
                    {
                        return BadRequest("model 3d not in valid format (only support glb, fbx), current is " + extension);
                    }
                    fileName += extension;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName); // save to database

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Asset3D asset = new Asset3D();

                    // fix tam
                    asset.Url = "https://localhost:5001/api/asset3d/" + userId + "/" + fileName;
                    asset.Id = assetId;
                    asset.Name = file.FileName;

                    // save data to db
                    userData.Assets.Add(asset);

                    await _userDbService.UpdateAsync(userId, userData);


                    return Ok(new { asset });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }


        [HttpGet("{userId}/{fileName}")]
        public async Task<IActionResult> downloadFile(string userId, string fileName)
        {
            try
            {
                var folderName = Path.Combine("Resources/Files", userId);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var filepath = Path.Combine(pathToSave, fileName);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filepath, out var contentType))
                {
                    contentType = "application/octest-stream";
                }

                var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                return File(bytes, contentType, Path.GetFileName(filepath));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveAll()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage
                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                userData.Assets.Clear();
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
