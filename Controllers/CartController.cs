using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using CartService.Model;
using CartService.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        //private IAmazonDynamoDB _ddbClient;
        private Cart _cart;

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

        // The Get method that doesn't take any parameters reads cart from session state, and will
        //  return an error if no session exists. Order service will call the next method (/Cart/session-id)
        // GET: api/Cart
        [HttpGet]
        public IActionResult Get()
        {
            LoadCart();

            //return new Task<IActionResult>(() => RedirectToPage("/"));
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.Id))
            {
                //TEMP test code - remove later!!
                _cart.Items.Add(new CartItem
                {
                    ProductId = Guid.Parse("dfc55ec1-2f35-4d11-9543-a52622ef00d5"),
                    Quantity = 2,
                    PriceWhenAdded = 23.99M,
                    DateAdded = DateTime.Now
                });
                _cart.Items.Add(new CartItem
                {
                    ProductId = Guid.Parse("a65bddcf-14b9-4990-a15e-6a9afa9679c6"),
                    Quantity = 1,
                    PriceWhenAdded = 19.95M,
                    DateAdded = DateTime.Now
                });

                return Ok(_cart);
            }

            return BadRequest("NoSessionStateLoaded");
        }

        // GET: api/Cart/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Cart
        [HttpPost]
        public void Post([FromBody]CartItem item)
        {
            LoadCart();

            item.DateAdded = DateTime.Now;
            _cart.Items.Add(item);

            HttpContext.Session.SetObject(CartKey, _cart);
        }
        
        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]CartItem item)
        {
            LoadCart();

            if (_cart.Items.First(x => x.ProductId == id) == null)
            {
                NotFound();
            }

            var existingItem = _cart.Items.First(x => x.ProductId == id);

            existingItem.Quantity = item.Quantity;
            existingItem.PriceWhenAdded = item.PriceWhenAdded;

            HttpContext.Session.SetObject(CartKey, _cart);
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
