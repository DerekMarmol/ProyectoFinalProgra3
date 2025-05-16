namespace SuperBodega.ECommerceWeb.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Total => Items.Sum(i => i.TotalPrice);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }
}