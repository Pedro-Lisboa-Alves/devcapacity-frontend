using System;
using System.Collections.Generic;

namespace DevCapacityWebApp.Models
{
    public enum EngineerCalendarDayType
    {
        Available,
        Vacations,
        Weekends,
        Absence,
        Assigned
    }

    public class EngineerCalendarDayDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
    }

    public class EngineerCalendarDto
    {
        public int EngineerCalendarId { get; set; }
        public int EngineerId { get; set; }
        public List<EngineerCalendarDayDto> Days { get; set; } = new();
    }

    public class EngineerDto
    {
        public int EngineerId { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public int DailyCapacity { get; set; }
        public int? TeamId { get; set; }
        public EngineerCalendarDto? EngineerCalendar { get; set; }
    }
}