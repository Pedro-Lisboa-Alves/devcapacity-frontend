using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;

namespace DevCapacityWebApp.Pages.Teams
{
    public class IndexModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public IndexModel(DevCapacityApiClient api) => _api = api;

        public List<Team> Teams { get; set; } = new();

        [BindProperty]
        public Team NewTeam { get; set; } = new();

        public async Task OnGetAsync()
        {
            Teams = await _api.GetTeamsAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _api.CreateTeamAsync(NewTeam);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteTeamAsync(id);
            return RedirectToPage();
        }
    }
}