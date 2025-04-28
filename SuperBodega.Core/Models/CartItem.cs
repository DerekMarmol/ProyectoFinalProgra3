namespace SuperBodega.Core.Models
{
    public class CartItem : EntityBase
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}