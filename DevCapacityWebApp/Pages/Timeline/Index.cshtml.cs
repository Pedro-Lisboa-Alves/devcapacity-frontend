using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Timeline
{
    public class IndexModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public IndexModel(DevCapacityApiClient api) => _api = api;

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Today.Year;

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public List<int> PageSizeOptions { get; set; } = new() { 5, 10, 20, 50 };

        // all engineers detailed (with calendar)
        public List<EngineerDto> EngineersDetailed { get; set; } = new();

        // engineers for current page
        public List<EngineerDto> PagedEngineers { get; set; } = new();

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        // convenience: months to render
        public List<int> MonthsToRender { get; set; } = Enumerable.Range(1, 12).ToList();

        public async Task<IActionResult> OnGetAsync()
        {
            // load basic engineers and then detailed per engineer (calendar)
            var basic = await _api.GetEngineersAsync();
            if (basic == null) basic = new List<Engineer>();

            var detailed = new List<EngineerDto>();
            foreach (var e in basic)
            {
                var ed = await _api.GetEngineerDetailedAsync(e.EngineerId);
                if (ed != null)
                {
                    detailed.Add(ed);
                }
                else
                {
                    // fallback: map basic to dto without calendar
                    detailed.Add(new EngineerDto
                    {
                        EngineerId = e.EngineerId,
                        Name = e.Name,
                        Role = e.Role,
                        TeamId = e.TeamId,
                        DailyCapacity = 0,
                        EngineerCalendar = null
                    });
                }
            }

            EngineersDetailed = detailed.OrderBy(x => x.Name).ToList();

            // pagination by engineers (rows)
            TotalCount = EngineersDetailed.Count;
            if (PageSize <= 0) PageSize = 10;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
            if (TotalPages == 0) TotalPages = 1;
            if (Page < 1) Page = 1;
            if (Page > TotalPages) Page = TotalPages;

            PagedEngineers = EngineersDetailed
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }

        // helper: build map date->type for engineer for selected year
        public string GetDayType(EngineerDto eng, DateTime date)
        {
            var days = eng.EngineerCalendar?.Days;
            if (days == null) return string.Empty;
            var found = days.FirstOrDefault(d => d.Date.Date == date.Date);
            return found?.Type ?? string.Empty;
        }
    }
}