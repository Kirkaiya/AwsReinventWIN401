using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CartService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Model;
using OrderService.Session;

namespace OrderService.Controllers
{
    [Produces("application/json")]
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        readonly IAmazonDynamoDB _ddbClient;
        readonly ILogger<OrdersController> _logger;

        public OrdersController(IAmazonDynamoDB dynamoDb, ILogger<OrdersController> logger)
        {
            _ddbClient = dynamoDb;
            _logger = logger;
        }

        // GET: api/Orders
        [HttpGet]
        public string Get()
        {
            return "{'Message': 'GET Orders functionality is not yet available'}";
        }

        // GET: api/Orders/1234-5678-9abc
        [HttpGet("{id}", Name = "Get")]
        [Authorize(Policy = "IsRegisteredUser")]
        public async Task<IActionResult> Get(Guid id)
        {
            var context = new DynamoDBContext(_ddbClient);
            var order = await context.LoadAsync<Order>(id);

            return order != null ? Ok(order) : (ObjectResult)NotFound(id);
        }
        
        // POST: api/Orders
        [HttpPost]
        [Authorize(Policy = "IsRegisteredUser")]
        public async Task<Order> Post([FromBody] CustomerInfo customerInfo)
        {
            //TODO get shopping cart from session, create an order with all those items, and the order info passed in, and return the order json
            var cart = HttpContext.Session.GetObject<Cart>("Cart");
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            _logger.LogInformation("New order created via POST by {0}", emailClaim?.Value);

            var order = new Order {
                CustomerInfo = customerInfo,
                OrderItems = cart.ItemsCollection(),
                Status = "Pending",
                UserCognitoEmail = emailClaim?.Value
            };

            var context = new DynamoDBContext(_ddbClient);
            await context.SaveAsync(order);

            return order;
        }

        // PUT: api/Orders/1234-5678-9abc
        [HttpPut("{id}")]
        [Authorize(Policy = "IsRegisteredUser")]
        public void Put(Guid id, [FromBody] Order value)
        {
            //TODO
        }

        // DELETE: api/ApiWithActions/1234-5678-9abc
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsRegisteredUser")]
        public async void Delete(Guid id)
        {
            var context = new DynamoDBContext(_ddbClient);
            await context.DeleteAsync<Order>(id);
        }

        // GET: api/orders/health
        [HttpGet]
        [Route("/api/orders/health")]
        public IActionResult Health()
        {
            return Ok();
        }
    }
}
