namespace Mazina_Backend.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Comment { get; set; } // Yeni eklendi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
