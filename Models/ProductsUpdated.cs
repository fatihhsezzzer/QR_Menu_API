using System.ComponentModel.DataAnnotations;

namespace Mazina_Backend.Models
{
    public class ProductUpdated
    {
        [Key]
        public int ProductId { get; set; }
        public  string Name { get; set; }
        public float Price { get; set; }
    }
}
