using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Services;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
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
        private readonly String connectionString;
        public FilesController(DatabaseController databaseController, IConfiguration configuration)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            connectionString = configuration["AzureFiles:ConnectionString"];
        }

        [Authorize]
        [HttpPost] // should return the link to this resource
        public async Task<IActionResult> uploadFile(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return BadRequest("file is null");
                }
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage
                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }


                if (file.Length > 0)
                {
                    var assetId = Guid.NewGuid().ToString();
                    var fileName = assetId + "";
                    string extension = Path.GetExtension(file.FileName);
                    fileName += extension;
                    var filePath = Path.Combine(userId, fileName);
                    var containerName = "users";

                    BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
                    await container.CreateIfNotExistsAsync();
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        ms.Position = 0;
                        var info = await container.UploadBlobAsync(filePath, ms);
                        var url = "https://museumfiles.blob.core.windows.net/" + containerName + "/" + filePath;
                        return Ok(url);
                    }
                }
                else
                {
                    return BadRequest("File is null");
                }
              
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
