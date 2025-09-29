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

        public List<Initiative> Initiatives { get; set; } = new();

        [BindProperty]
        public Initiative NewInitiative { get; set; } = new();

        public async Task OnGetAsync()
        {
            Initiatives = await _api.GetInitiativesAsync();
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