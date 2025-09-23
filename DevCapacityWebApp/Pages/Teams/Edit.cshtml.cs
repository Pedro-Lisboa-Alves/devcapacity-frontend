using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Teams
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public Team Team { get; set; } = new();

        // engineers assigned to this team
        public List<Engineer> AssignedEngineers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var t = await _api.GetTeamAsync(id);
            if (t == null) return RedirectToPage("/Teams/Index");

            Team = t;

            var all = await _api.GetEngineersAsync();
            AssignedEngineers = all.Where(e => e.TeamId.HasValue && e.TeamId.Value == Team.TeamId).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var all = await _api.GetEngineersAsync();
                AssignedEngineers = all.Where(e => e.TeamId.HasValue && e.TeamId.Value == Team.TeamId).ToList();
                return Page();
            }

            await _api.UpdateTeamAsync(Team.TeamId, Team);
            return RedirectToPage("/Teams/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteTeamAsync(id);
            return RedirectToPage("/Teams/Index");
        }
    }
}