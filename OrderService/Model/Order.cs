using Amazon.DynamoDBv2.DataModel;
using CartService.Model;
using System;
using System.Collections.Generic;

namespace OrderService.Model
{
    [DynamoDBTable("OrderTable")]
    public class Order
    {
        [DynamoDBHashKey]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        public CustomerInfo CustomerInfo { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string UserCognitoEmail { get; set; }

        public string Status { get; set; }

        public IEnumerable<CartItem> OrderItems { get; set; }
    }
}
