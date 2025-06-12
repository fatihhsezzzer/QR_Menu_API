using System.ComponentModel.DataAnnotations;

namespace Mazina_Backend.Models
{
    public class DailyProductsUpdated
    {
        [Key]
        public int ProductId { get; set; }
        public float OldPrice { get; set; }
        public float NewPrice { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

    }
}
