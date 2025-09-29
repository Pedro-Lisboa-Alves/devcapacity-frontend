using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public TaskItem Task { get; set; } = new();

        public List<Initiative> Initiatives { get; set; } = new();
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
            Engineers = await _api.GetEngineersAsync();
            Assignments = await _api.GetAssignmentsForTaskAsync(Task.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Initiatives = await _api.GetInitiativesAsync();
                Engineers = await _api.GetEngineersAsync();
                Assignments = await _api.GetAssignmentsForTaskAsync(Task.Id);
                return Page();
            }

            await _api.UpdateTaskAsync(Task.Id, Task);
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
            // remain on same task page
            return Redirect(Request.Path + Request.QueryString);
        }
    }
}