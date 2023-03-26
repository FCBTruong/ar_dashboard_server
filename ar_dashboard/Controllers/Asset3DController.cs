using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Asset3DController : ControllerBase
    {
        public Asset3DController()
        {

        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateAsset3D()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources/Model3D", userId);
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                Console.WriteLine("path to save" + pathToSave);

                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    fileName += extension;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName); // save to database

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
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
    }
}
