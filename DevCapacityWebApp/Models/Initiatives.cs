using System;
using System.Collections.Generic;

namespace DevCapacityWebApp.Models
{
    public class Initiatives
    {
        // Nome dos campos alinhados com o DTO da API
        public int InitiativeId { get; set; }
        public string? Name { get; set; }
        public int? ParentInitiative { get; set; }
        public int Status { get; set; }
        public int PDs { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        // Use List<int> para facilitar o binding e manipulação local
        public List<int> TaskIds { get; set; } = new List<int>();
    }
}