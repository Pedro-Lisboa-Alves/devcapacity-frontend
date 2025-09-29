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
        // nullable dates so inputs can be empty
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // Use List<int> para facilitar o binding e manipulação local
        public List<int> TaskIds { get; set; } = new List<int>();
    }
}