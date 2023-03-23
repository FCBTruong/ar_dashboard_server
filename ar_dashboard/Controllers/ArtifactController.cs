using System;
using System.Linq;
using System.Threading.Tasks;
using ar_dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtifactController:ControllerBase
    {
        private readonly IDocumentClient _documentClient;
        readonly String databaseId;
        readonly String collectionId;

        public IConfiguration Configuration { get; set; }


        public ArtifactController(IDocumentClient documentClient, IConfiguration configuration)
        {
            _documentClient = documentClient;
            Configuration = configuration;

            databaseId = Configuration["DatabaseId"];
            collectionId = "Items";

            BuildCollection().Wait();
        }

        private async Task BuildCollection()
        {
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId });
        }

        [HttpGet]
        [Route("get-all")]
        public IQueryable<Artifact> Get()
        {
            return _documentClient.CreateDocumentQuery<Artifact>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                new FeedOptions { MaxItemCount = 20});
        }

        [Route("get")]
        public IQueryable<Artifact> Get(string id)
        {
            return _documentClient.CreateDocumentQuery<Artifact>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                new FeedOptions { MaxItemCount = 1 }).Where((i) => i.Id == id);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Post([FromBody] Artifact item)
        {
            var response = await _documentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), item);
            return Ok();
        }


    }
}
