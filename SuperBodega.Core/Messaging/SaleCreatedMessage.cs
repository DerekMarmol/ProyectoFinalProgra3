using System;
using System.Collections.Generic;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Messaging
{
    public class SaleCreatedMessage
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public string Reference { get; set; }
        public List<SaleDetailDto> SaleDetails { get; set; }
    }
}