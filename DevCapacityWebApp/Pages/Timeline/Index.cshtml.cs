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

        // ADICIONADO: meses seleccionados (multi-select). Default: mês actual
        [BindProperty(SupportsGet = true)]
        public List<int> SelectedMonths { get; set; } = new List<int> { DateTime.Today.Month };

        public List<int> PageSizeOptions { get; set; } = new() { 5, 10, 20, 50 };

        public List<EngineerDto> EngineersDetailed { get; set; } = new();
        public List<EngineerDto> PagedEngineers { get; set; } = new();

        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        // now computed from SelectedMonths (or fallback to all)
        public List<int> MonthsToRender { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var basic = await _api.GetEngineersAsync() ?? new List<Engineer>();

            var detailed = new List<EngineerDto>();
            foreach (var e in basic)
            {
                var ed = await _api.GetEngineerDetailedAsync(e.EngineerId);
                if (ed != null) detailed.Add(ed);
                else detailed.Add(new EngineerDto
                {
                    EngineerId = e.EngineerId,
                    Name = e.Name,
                    Role = e.Role,
                    TeamId = e.TeamId,
                    DailyCapacity = 0,
                    EngineerCalendar = null
                });
            }

            EngineersDetailed = detailed.OrderBy(x => x.Name).ToList();

            // determine months to render from SelectedMonths (if any), otherwise all months
            if (SelectedMonths != null && SelectedMonths.Any())
            {
                MonthsToRender = SelectedMonths
                    .Where(m => m >= 1 && m <= 12)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();
            }
            else
            {
                MonthsToRender = Enumerable.Range(1, 12).ToList();
            }

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

        public string GetDayType(EngineerDto eng, DateTime date)
        {
            var days = eng.EngineerCalendar?.Days;
            if (days == null) return string.Empty;
            var found = days.FirstOrDefault(d => d.Date.Date == date.Date);
            return found?.Type ?? string.Empty;
        }
    }
}