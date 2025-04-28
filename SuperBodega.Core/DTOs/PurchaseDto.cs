using System;
using System.Collections.Generic;

namespace SuperBodega.Core.DTOs
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Reference { get; set; }
        public List<PurchaseDetailDto> PurchaseDetails { get; set; }
    }
}