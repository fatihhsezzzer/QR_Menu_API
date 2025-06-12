namespace Mazina_Backend.Models
{
    public class SPLog
    {
        public int Id { get; set; }  // Identity (Auto Increment)
        public string ProcedureName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime ExecutionTime { get; set; } = DateTime.Now;
    }

}
