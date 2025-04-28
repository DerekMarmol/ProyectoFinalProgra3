using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public class Product : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<SaleDetail> SaleDetails { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}