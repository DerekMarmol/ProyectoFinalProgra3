using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public class Customer : EntityBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}