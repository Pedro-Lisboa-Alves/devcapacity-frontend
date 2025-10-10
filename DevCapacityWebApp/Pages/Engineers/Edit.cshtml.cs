using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Engineers
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public Engineer Engineer { get; set; } = new();

        // added: teams for the select
        public List<Team> Teams { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var e = await _api.GetEngineerAsync(id);
            if (e == null) return RedirectToPage("/Engineers/Index");
            Engineer = e;
            Teams = await _api.GetTeamsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Teams = await _api.GetTeamsAsync();
                return Page();
            }
            await _api.UpdateEngineerAsync(Engineer.EngineerId, Engineer);
            return RedirectToPage("/Engineers/Index");
        }
    }
}