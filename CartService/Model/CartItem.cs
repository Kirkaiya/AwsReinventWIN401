using System;

namespace CartService.Model
{
    public class CartItem
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public DateTime DateAdded { get; set; }

        public decimal PriceWhenAdded { get; set; }
    }
}
