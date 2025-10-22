using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Engineers
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public Engineer Engineer { get; set; } = new();
        public EngineerDto? EngineerDetailed { get; set; }

        public List<EngineerCalendarDayDto> CalendarDays { get; set; } = new();

        // filter (querystring) - default Assigned
        [BindProperty(SupportsGet = true)]
        public string FilterType { get; set; } = EngineerCalendarDayType.Assigned.ToString();

        // pagination (querystring)
        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public List<string> AvailableTypes { get; set; } = Enum.GetNames(typeof(EngineerCalendarDayType)).ToList();

        // results for the current page
        public List<EngineerCalendarDayDto> PagedCalendarDays { get; set; } = new();

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public List<int> PageSizeOptions { get; set; } = new() { 5, 10, 20, 50 };

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // determine effective id from route/query/form (existing fallback behaviour)
            var effectiveId = id ?? 0;
            if (effectiveId <= 0)
            {
                var formId = Request.Query["Engineer.EngineerId"].ToString();
                if (string.IsNullOrEmpty(formId))
                {
                    formId = Request.Query["id"].ToString();
                }
                if (!string.IsNullOrEmpty(formId) && int.TryParse(formId, out var parsed))
                {
                    effectiveId = parsed;
                }
            }

            if (effectiveId <= 0)
                return Page();

            // load basic engineer
            var basic = await _api.GetEngineerAsync(effectiveId);
            if (basic != null)
            {
                Engineer = basic;
            }

            // load detailed (calendar)
            EngineerDetailed = await _api.GetEngineerDetailedAsync(effectiveId);

            CalendarDays = EngineerDetailed?.EngineerCalendar?.Days
                .OrderBy(d => d.Date)
                .ToList() ?? new List<EngineerCalendarDayDto>();

            // apply filter
            ApplyFilter();

            // pagination
            TotalCount = CalendarDays.Count;
            if (PageSize <= 0) PageSize = 10;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (Page < 1) Page = 1;
            if (Page > TotalPages) Page = TotalPages;

            PagedCalendarDays = CalendarDays
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }

        // apply filter to CalendarDays using FilterType
        private void ApplyFilter()
        {
            if (!string.IsNullOrEmpty(FilterType))
            {
                CalendarDays = CalendarDays
                    .Where(d => string.Equals(d.Type ?? string.Empty, FilterType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }
    }
}