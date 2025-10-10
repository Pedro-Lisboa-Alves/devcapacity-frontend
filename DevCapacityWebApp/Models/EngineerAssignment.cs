using System;

namespace DevCapacityWebApp.Models
{
    // Alinhado com o DTO da API
    public class EngineerAssignment
    {
        public int AssignmentId { get; set; }
        public int EngineerId { get; set; }
        public int TaskId { get; set; }
        public int CapacityShare { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}