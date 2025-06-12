namespace Mazina_Backend.Models
{
    public class GoogleReviewClick
    {
        public int Id { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow; // Zaman kaydı
    }
}
