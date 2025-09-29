using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Initiatives
{
    public class IndexModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public IndexModel(DevCapacityApiClient api) => _api = api;

        public List<DevCapacityWebApp.Models.Initiatives> Initiatives { get; set; } = new();
        public List<Status> Statuses { get; set; } = new();

        [BindProperty]
        public DevCapacityWebApp.Models.Initiatives NewInitiative { get; set; } = new();

        public async Task OnGetAsync()
        {
            Initiatives = await _api.GetInitiativesAsync();
            Statuses = await _api.GetStatusesAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _api.CreateInitiativeAsync(NewInitiative);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteInitiativeAsync(id);
            return RedirectToPage();
        }
    }
}