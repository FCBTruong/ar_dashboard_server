using System;
using System.Threading.Tasks;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IAdminDbService _adminDbService;
        public PublicController(DatabaseController databaseController)
        {
            _adminDbService = databaseController.AdminDbService ?? throw new ArgumentNullException(nameof(databaseController));
        }

        [HttpPost]
        public async Task<IActionResult> RequestPublish()
        {
            try
            {
                var adminModel = await _adminDbService.GetAsync();
                

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
