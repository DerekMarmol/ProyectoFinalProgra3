using System.Collections.Generic;

namespace SuperBodega.Core.Models
{
    public class Category : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}