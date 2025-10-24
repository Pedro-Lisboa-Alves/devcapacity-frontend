using System;
using System.Collections.Generic;

namespace DevCapacityWebApp.Models
{
    // Alinha com o DTO da API
    public class Tasks
    {
        public int TaskId { get; set; }
        public string? Name { get; set; }
        public int Initiative { get; set; }
        public int Status { get; set; }
        public int PDs { get; set; }

        // ADICIONADO: limite de recursos para a task
        public int MaxResources { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // assignment ids
        public List<int> AssignmentIds { get; set; } = new List<int>();
    }
}