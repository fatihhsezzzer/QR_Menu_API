using System.ComponentModel.DataAnnotations;

namespace Mazina_Backend.Models
{
    public class DailyProductsInserted
    {
        [Key]
        public int ProductId { get; set; }
        public float Price { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
