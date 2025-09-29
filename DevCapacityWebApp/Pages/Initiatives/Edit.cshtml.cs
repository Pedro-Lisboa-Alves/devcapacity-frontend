using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Initiatives
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public DevCapacityWebApp.Models.Initiatives Initiative { get; set; } = new();

        public List<TaskItem> Tasks { get; set; } = new();
        public List<Status> Statuses { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var it = await _api.GetInitiativeAsync(id);
            if (it == null) return RedirectToPage("/Initiatives/Index");
            Initiative = it;

            var allTasks = await _api.GetTasksAsync();
            Tasks = allTasks.Where(t => t.InitiativeId.HasValue && t.InitiativeId.Value == Initiative.InitiativeId).ToList();

            Statuses = await _api.GetStatusesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var allTasks = await _api.GetTasksAsync();
                Tasks = allTasks.Where(t => t.InitiativeId.HasValue && t.InitiativeId.Value == Initiative.InitiativeId).ToList();
                Statuses = await _api.GetStatusesAsync();
                return Page();
            }

            await _api.UpdateInitiativeAsync(Initiative.InitiativeId, Initiative);
            return RedirectToPage("/Initiatives/Index");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteInitiativeAsync(id);
            return RedirectToPage("/Initiatives/Index");
        }
    }
}