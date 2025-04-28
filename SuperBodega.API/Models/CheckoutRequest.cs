namespace SuperBodega.API.Models
{
    public class CheckoutRequest
    {
        // Si el cliente ya existe
        public int? CustomerId { get; set; }
        
        // Si es un nuevo cliente
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}