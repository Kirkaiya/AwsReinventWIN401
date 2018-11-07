using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;

namespace ProductService.Model
{
    [DynamoDBTable("ProductTable")]
    public class Product
    {
        public Product()
        {
            ProductId = Guid.NewGuid().ToString();
        }

        [DynamoDBHashKey]
        public string ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int AvailableStock { get; set; }

        public string PictureFilename { get; set; }

        public List<string> Categories { get; set; } = new List<string>();
    }
}
