using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Services;
using DevCapacityWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public IndexModel(DevCapacityApiClient api) => _api = api;

        public List<TaskItem> Tasks { get; set; } = new();
        public List<Initiative> Initiatives { get; set; } = new();

        [BindProperty]
        public TaskItem NewTask { get; set; } = new();

        public async Task OnGetAsync()
        {
            Tasks = await _api.GetTasksAsync();
            Initiatives = await _api.GetInitiativesAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _api.CreateTaskAsync(NewTask);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _api.DeleteTaskAsync(id);
            return RedirectToPage();
        }
    }
}