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

        // detailed DTO including calendar days
        public EngineerDto? EngineerDetailed { get; set; }

        public List<EngineerCalendarDayDto> CalendarDays { get; set; } = new();

        // filter (querystring) - default Assigned
        [BindProperty(SupportsGet = true)]
        public string FilterType { get; set; } = EngineerCalendarDayType.Assigned.ToString();

        public List<string> AvailableTypes { get; set; } = Enum.GetNames(typeof(EngineerCalendarDayType)).ToList();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // fallback: aceitar id via route/query ou via Engineer.EngineerId vindo do form
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

            // se ainda for inválido, apenas renderiza a página (ou redireccione conforme desejado)
            if (effectiveId <= 0)
                return Page();

            // load basic engineer (existing behaviour)
            var basic = await _api.GetEngineerAsync(effectiveId);
            if (basic != null)
            {
                Engineer = basic;
            }

            // load detailed DTO (calendar)
            EngineerDetailed = await _api.GetEngineerDetailedAsync(effectiveId);

            CalendarDays = EngineerDetailed?.EngineerCalendar?.Days
                .OrderBy(d => d.Date)
                .ToList() ?? new List<EngineerCalendarDayDto>();

            ApplyFilter();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // garantir que temos o id vindo do form; se não, tentar query/route
            if (Engineer == null || Engineer.EngineerId == 0)
            {
                var qs = Request.Form["Engineer.EngineerId"].ToString();
                if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out var fid))
                    Engineer.EngineerId = fid;
                else
                {
                    var q = Request.Query["id"].ToString();
                    if (!string.IsNullOrEmpty(q) && int.TryParse(q, out var qid))
                        Engineer.EngineerId = qid;
                    else if (RouteData.Values.TryGetValue("id", out var ridObj) && int.TryParse(ridObj?.ToString(), out var rid))
                        Engineer.EngineerId = rid;
                }
            }

            if (!ModelState.IsValid)
            {
                // reload detailed info for rendering the page again
                EngineerDetailed = await _api.GetEngineerDetailedAsync(Engineer.EngineerId);
                CalendarDays = EngineerDetailed?.EngineerCalendar?.Days.OrderBy(d => d.Date).ToList() ?? new List<EngineerCalendarDayDto>();
                ApplyFilter();
                return Page();
            }

            await _api.UpdateEngineerAsync(Engineer.EngineerId, Engineer);
            return RedirectToPage(new { id = Engineer.EngineerId });
        }

        // apply filter to CalendarDays using FilterType
        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(FilterType)) return;
            CalendarDays = CalendarDays
                .Where(d => string.Equals(d.Type ?? string.Empty, FilterType, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}