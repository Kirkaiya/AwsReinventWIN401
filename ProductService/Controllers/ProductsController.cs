using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductService.Model;

namespace ProductService.Controllers
{
    [Produces("application/json")]
    [Route("api/Products")]
    [EnableCors("AllowAllOrigins")]
    public class ProductsController : Controller
    {
        readonly IAmazonDynamoDB _ddbClient;
        readonly ILogger<ProductsController> _logger;

        public ProductsController(IAmazonDynamoDB dynamoDb, ILogger<ProductsController> logger)
        {
            _ddbClient = dynamoDb;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            var context = new DynamoDBContext(_ddbClient);
            var search = context.ScanAsync<Product>(new List<ScanCondition>());
            var results = await search.GetRemainingAsync();

            return results;
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(string id)
        {
            var context = new DynamoDBContext(_ddbClient);
            var product = await context.LoadAsync<Product>(id);

            return product != null ? Ok(product) : (ObjectResult)NotFound(id);
        }
        
        // POST: api/Products
        [HttpPost]
        [Authorize(Policy = "IsSiteAdmin")]
        public async Task<string> Post([FromBody]Product product)
        {
            var context = new DynamoDBContext(_ddbClient);
            await context.SaveAsync(product);

            _logger.LogInformation("New product {0} created by user {1}", product.Name, User.Identity.Name);

            return product.ProductId;
        }
        
        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize(Policy = "IsSiteAdmin")]
        public async Task<IActionResult> Put(int id, [FromBody]Product product)
        {
            var context = new DynamoDBContext(_ddbClient);
            await context.SaveAsync(product);

            return Ok();
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsSiteAdmin")]
        public async void Delete(string id)
        {
            var context = new DynamoDBContext(_ddbClient);
            await context.DeleteAsync<Product>(id);
        }
    }
}
