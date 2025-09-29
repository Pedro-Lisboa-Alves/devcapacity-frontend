using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public DevCapacityWebApp.Models.Tasks Task { get; set; } = new();

        public List<DevCapacityWebApp.Models.Initiatives> Initiatives { get; set; } = new();
        public List<Status> Statuses { get; set; } = new();
        public List<Engineer> Engineers { get; set; } = new();
        public List<EngineerAssignment> Assignments { get; set; } = new();

        [BindProperty]
        public EngineerAssignment NewAssignment { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var t = await _api.GetTaskAsync(id);
            if (t == null) return RedirectToPage("/Tasks/Index");
            Task = t;

            Initiatives = await _api.GetInitiativesAsync();
            Statuses = await _api.GetStatusesAsync();
            Engineers = await _api.GetEngineersAsync();
            Assignments = await _api.GetAssignmentsForTaskAsync(Task.TaskId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Initiatives = await _api.GetInitiativesAsync();
                Statuses = await _api.GetStatusesAsync();
                Engineers = await _api.GetEngineersAsync();
                Assignments = await _api.GetAssignmentsForTaskAsync(Task.TaskId);
                return Page();
            }

            await _api.UpdateTaskAsync(Task.TaskId, Task);
            return RedirectToPage("/Tasks/Index");
        }

        public async Task<IActionResult> OnPostAddAssignmentAsync()
        {
            if (NewAssignment == null || NewAssignment.EngineerId == 0) return RedirectToPage();
            await _api.CreateAssignmentAsync(NewAssignment);
            return RedirectToPage(new { id = NewAssignment.TaskId });
        }

        public async Task<IActionResult> OnPostDeleteAssignmentAsync(int id)
        {
            await _api.DeleteAssignmentAsync(id);
            return Redirect(Request.Path + Request.QueryString);
        }
    }
}