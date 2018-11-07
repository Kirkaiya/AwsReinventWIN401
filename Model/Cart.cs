using System;
using System.Collections.Generic;

namespace CartService.Model
{
    public class Cart
    {
        public IDictionary<Guid, CartItem> Items { get; set; }

        public Cart()
        {
            Items = new Dictionary<Guid, CartItem>();
        }

        public IEnumerable<CartItem> ItemsCollection() => Items.Values;

        public void Add(CartItem item)
        {
            if (Items.ContainsKey(item.ProductId))
                Items[item.ProductId] = item;
            else
                Items.Add(item.ProductId, item);
        }

        public void Remove(Guid ProductId) => Items.Remove(ProductId);

        public bool Contains(Guid ProductId) => Items.ContainsKey(ProductId);
    }
}
