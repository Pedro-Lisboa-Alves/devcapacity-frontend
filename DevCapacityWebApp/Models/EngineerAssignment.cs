namespace DevCapacityWebApp.Models
{
    public class EngineerAssignment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int EngineerId { get; set; }
        public string? Role { get; set; }
        public decimal? Hours { get; set; }
    }
}