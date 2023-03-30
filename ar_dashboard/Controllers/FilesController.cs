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
    public class FilesController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        public FilesController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));

        }

        [Authorize]
        [HttpPost] // should return the link to this resource
        public async Task<IActionResult> uploadFile(IFormFile file)
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

                var folderName = Path.Combine("Resources/Model3D", userId);
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
                    fileName += extension;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var link = "https://localhost:5001/api/files/" + userId + "/" + fileName;

                    return Ok(new { link });
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
                var folderName = Path.Combine("Resources/Model3D", userId);
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
    }
}
