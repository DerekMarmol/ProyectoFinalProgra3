using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public class Supplier : EntityBase
    {
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }
}