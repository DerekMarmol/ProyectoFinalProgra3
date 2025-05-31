using System;
using System.Collections.Generic;

namespace SuperBodega.Core.DTOs.Reports
{
    public class CustomersReportDto
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; }
        public decimal AverageCustomerValue { get; set; }
        public List<TopCustomerDto> TopCustomers { get; set; } = new List<TopCustomerDto>();
        public List<CustomerGrowthDto> CustomerGrowth { get; set; } = new List<CustomerGrowthDto>();
    }

    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    public class CustomerGrowthDto
    {
        public DateTime Month { get; set; }
        public int NewCustomers { get; set; }
        public int TotalCustomers { get; set; }
    }
}