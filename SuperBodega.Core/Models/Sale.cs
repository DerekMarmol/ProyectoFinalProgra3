using System;
using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public enum SaleStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Sale : EntityBase
    {
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Pending;
        public string Reference { get; set; }
        public ICollection<SaleDetail> SaleDetails { get; set; }
    }
}