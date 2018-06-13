using System.Collections.Generic;

namespace CartService.Model
{
    public class Cart
    {
        public IList<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }

        public void Add(CartItem item) => Items.Add(item);
    }
}
