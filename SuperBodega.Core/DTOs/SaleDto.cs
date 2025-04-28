using System;
using System.Collections.Generic;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; }
        public string Reference { get; set; }
        public List<SaleDetailDto> SaleDetails { get; set; }
    }
}