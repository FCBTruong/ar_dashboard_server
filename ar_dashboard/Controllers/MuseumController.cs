using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuseumController : ControllerBase
    {
        private readonly IDocumentClient _documentClient;
        readonly string databaseId;
        readonly string collectionId;

        public IConfiguration Configuration { get; set; }
        public MuseumController(IDocumentClient documentClient, IConfiguration configuration)
        {
            _documentClient = documentClient;
            Configuration = configuration;

            databaseId = Configuration["DatabaseId"];
            collectionId = "museums";

        //    BuildCollection().Wait();
        }

       
  



        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateMuseum()
        {
            try
            {
                string userId = "truong123"; // test
                var userData = _documentClient.CreateDocumentQuery<UserData>(UriFactory.CreateDocumentCollectionUri(databaseId, userId));

                var newMuseum = new Museum();
        

                await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = userId });

                var response = await _documentClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, userId), newMuseum);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
