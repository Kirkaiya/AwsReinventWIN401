using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using CartService.Model;
using CartService.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.DynamoDb;
using Newtonsoft.Json;

namespace CartService.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    [EnableCors("AllowAllOrigins")]
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        private readonly IAmazonDynamoDB _ddbClient;
        private Cart _cart;

        public CartController(IAmazonDynamoDB dynamoDB)
        {
            _ddbClient = dynamoDB;
        }

        private void LoadCart()
        {
            HttpContext.Session.LoadAsync().Wait();

            if (HttpContext.Session.IsAvailable && HttpContext.Session.Keys.Contains(CartKey))
            {
                _cart = HttpContext.Session.GetObject<Cart>(CartKey);
            }
            else
            {
                _cart = new Cart();
                HttpContext.Session.SetObject(CartKey, _cart);
            }
        }

        // GET: api/cart/health
        [HttpGet]
        [Route("/api/cart/health")]
        public IActionResult Health()
        {
            return Ok();
        }
        
        // The Get method that doesn't take any parameters reads cart from session state, and will
        //  return an error if no session exists. Order service will call the next method (/Cart/session-id)
        // GET: api/Cart
        [HttpGet]
        public IActionResult Get()
        {
            LoadCart();

            if (!string.IsNullOrWhiteSpace(HttpContext.Session.Id))
            {
                //TEMP test code - remove later!!
                if (!_cart.Items.Any())
                {
                    _cart.Items.Add(new CartItem
                    {
                        ProductId = Guid.Parse("dfc55ec1-2f35-4d11-9543-a52622ef00d5"),
                        Name = ".NET Bot Black Hoodie",
                        Quantity = 2,
                        PriceWhenAdded = 23.99M,
                        DateAdded = DateTime.Now
                    });
                    _cart.Items.Add(new CartItem
                    {
                        ProductId = Guid.Parse("a65bddcf-14b9-4990-a15e-6a9afa9679c6"),
                        Name = ".NET Black & White Mug",
                        Quantity = 1,
                        PriceWhenAdded = 19.95M,
                        DateAdded = DateTime.Now
                    });

                    HttpContext.Session.SetObject(CartKey, _cart);
                }
                
                return Ok(_cart);
            }

            return BadRequest("NoSessionIdForCurrentRequest");
        }

        // GET: api/Cart/5
        [HttpGet("{id}", Name = "Get")]
        [Authorize("IsRegisteredUser")]
        public async Task<IActionResult> Get(string id)
        {
            if (!Guid.TryParse(id, out Guid SessionId))
            {
                return BadRequest("InvalidIdIsNotGuid");
            }

            var options = new DynamoDbCacheOptions { TableName = "TechSummitSessionState", TtlAttribute = "TTL" };
            var cache = new DynamoDbCache(options, _ddbClient);
            var sessionBytes = await cache.GetAsync(id);
            
            if (sessionBytes == null)
            {
                return Ok(new Cart());
            }

            var sessionString = System.Text.Encoding.UTF8.GetString(sessionBytes);
            sessionString = sessionString.Substring(sessionString.IndexOf("{\"Items\":"));

            var cart = JsonConvert.DeserializeObject<Cart>(sessionString);

            return Ok(cart);
        }
        
        // POST: api/Cart
        [HttpPost]
        public void Post([FromBody]CartItem item)
        {
            LoadCart();

            item.DateAdded = DateTime.Now;
            _cart.Add(item);

            HttpContext.Session.SetObject(CartKey, _cart);
        }
        
        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]CartItem item)
        {
            LoadCart();

            if (!_cart.Items.Any(x => x.ProductId == id))
            {
                NotFound();
            }

            var existingItem = _cart.Items.First(x => x.ProductId == id);

            existingItem.Quantity = item.Quantity;
            existingItem.PriceWhenAdded = item.PriceWhenAdded;

            HttpContext.Session.SetObject(CartKey, _cart);
            Ok();
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            HttpContext.Session.Remove(CartKey);
        }
    }
}
