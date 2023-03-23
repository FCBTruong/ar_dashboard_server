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
    public class MuseumController : Controller
    {
        private readonly IDocumentClient _documentClient;
        readonly String databaseId;
        readonly String collectionId;

        public IConfiguration Configuration { get; set; }
        public MuseumController(IDocumentClient documentClient, IConfiguration configuration)
        {
            _documentClient = documentClient;
            Configuration = configuration;

            databaseId = Configuration["DatabaseId"];
            collectionId = "museums";

            BuildCollection().Wait();
        }

        private async Task BuildCollection()
        {
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId });
        }



        [HttpPost]
        [Route("createMuseum")]
        public async Task<IActionResult> CreateMuseum()
        {
            string userId = "truong123"; // test
            var newMuseum = new Museum();
            var userDb = databaseId + "/" + userId;
            var response = await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(userDb, collectionId), newMuseum);
            return Ok();
        }
    }
}
