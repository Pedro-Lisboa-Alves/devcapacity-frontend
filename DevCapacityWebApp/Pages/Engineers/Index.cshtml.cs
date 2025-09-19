using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;

namespace DevCapacityWebApp.Pages.Engineers
{
    public class IndexModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public IndexModel(DevCapacityApiClient api) => _api = api;

        public List<Engineer> Engineers { get; set; } = new();

        // added: list of teams for the create select
        public List<Team> Teams { get; set; } = new();

        [BindProperty]
        public Engineer NewEngineer { get; set; } = new();

        public async Task OnGetAsync()
        {
            Engineers = await _api.GetEngineersAsync();
            Teams = await _api.GetTeamsAsync(); // load teams for the select
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _api.CreateEngineerAsync(NewEngineer);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteEngineerAsync(id);
            return RedirectToPage();
        }
    }
}