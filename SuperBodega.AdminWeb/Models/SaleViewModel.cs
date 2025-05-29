// Models/SaleViewModel.cs
namespace SuperBodega.AdminWeb.Models
{
    public class SaleViewModel
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public string Reference { get; set; }
        public List<SaleDetailViewModel> SaleDetails { get; set; }
    }

    public class SaleDetailViewModel
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}