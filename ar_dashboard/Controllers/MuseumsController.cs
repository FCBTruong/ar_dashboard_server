using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuseumsController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public MuseumsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
        }

        // GET api/museums/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var userData = (UserData) await _cosmosDbService.GetAsync(id);

                if (userData == null)
                {
                    return BadRequest("user is null");
                }

                var museum = userData.Museums.Where((m) => m.Id == id);
                if(museum == null)
                {
                    return BadRequest("museum is null");
                }
                return Ok(museum);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        // POST api/museums/
        [HttpPost("{id}")]
        public async Task<IActionResult> Create([FromBody] Museum item)
        {
            try
            {
                /* item.Id = Guid.NewGuid().ToString();

                 var userData = await _cosmosDbService.GetAsync(id);

                 userData.mu
                 await _cosmosDbService.AddAsync(item); */
                return Ok();

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
