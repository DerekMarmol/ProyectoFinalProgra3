using System;
using System.Collections.Generic;

namespace SuperBodega.AdminWeb.Models.Reports
{
    public class CustomersReportViewModel
    {
        public int TotalCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; }
        public decimal AverageCustomerValue { get; set; }
        public List<TopCustomerViewModel> TopCustomers { get; set; } = new List<TopCustomerViewModel>();
        public List<CustomerGrowthViewModel> CustomerGrowth { get; set; } = new List<CustomerGrowthViewModel>();
    }

    public class TopCustomerViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    public class CustomerGrowthViewModel
    {
        public DateTime Month { get; set; }
        public int NewCustomers { get; set; }
        public int TotalCustomers { get; set; }
    }
}