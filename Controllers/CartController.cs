using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    public class CartController : Controller
    {
        IAmazonDynamoDB _ddbClient;

        public CartController(IAmazonDynamoDB dynamoDb)
        {
            _ddbClient = dynamoDb;
        }

        // GET: api/Cart
        [HttpGet]
        public Task<IActionResult> Get()
        {
            if (!ControllerContext.HttpContext.Session.IsAvailable)
            {
                return new Task<IActionResult>(() => RedirectToPage("/"));
            }

            var sessionId = ControllerContext.HttpContext.Session.Id;

            return new Task<IActionResult>(() => Ok());
        }

        // GET: api/Cart/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Cart
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
