using System;
using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public class Purchase : EntityBase
    {
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public decimal TotalAmount { get; set; }
        public string Reference { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}