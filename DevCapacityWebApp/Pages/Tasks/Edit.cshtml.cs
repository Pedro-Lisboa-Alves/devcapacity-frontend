using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCapacityWebApp.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly DevCapacityApiClient _api;
        public EditModel(DevCapacityApiClient api) => _api = api;

        [BindProperty]
        public DevCapacityWebApp.Models.Tasks Task { get; set; } = new DevCapacityWebApp.Models.Tasks();

        public List<DevCapacityWebApp.Models.Initiatives> Initiatives { get; set; } = new();
        public List<DevCapacityWebApp.Models.Status> Statuses { get; set; } = new();
        public List<Engineer> Engineers { get; set; } = new();
        public List<EngineerAssignment> Assignments { get; set; } = new();

        [BindProperty]
        public EngineerAssignment NewAssignment { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0) return RedirectToPage("/Tasks/Index");

            var t = await _api.GetTaskAsync(id);
            if (t == null) return RedirectToPage("/Tasks/Index");

            Task = t;
            Initiatives = await _api.GetInitiativesAsync();
            Statuses = await _api.GetStatusesAsync();
            Engineers = await _api.GetEngineersAsync();
            Assignments = await _api.GetAssignmentsForTaskAsync(Task.TaskId) ?? new List<EngineerAssignment>();

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
            if (NewAssignment == null)
                return Redirect(Request.Path + Request.QueryString);

            // resolve TaskId fallback
            if (NewAssignment.TaskId == 0)
            {
                var formTask = Request.Form["NewAssignment.TaskId"].ToString();
                if (!string.IsNullOrEmpty(formTask) && int.TryParse(formTask, out var fid))
                    NewAssignment.TaskId = fid;
                else if (Task != null && Task.TaskId != 0)
                    NewAssignment.TaskId = Task.TaskId;
                else if (RouteData.Values.TryGetValue("id", out var ridObj) && int.TryParse(ridObj?.ToString(), out var rid))
                    NewAssignment.TaskId = rid;
            }

            // resolve EngineerId fallback
            if (NewAssignment.EngineerId == 0)
            {
                var formEng = Request.Form["NewAssignment.EngineerId"].ToString();
                if (!string.IsNullOrEmpty(formEng) && int.TryParse(formEng, out var eid))
                    NewAssignment.EngineerId = eid;
            }

            if (NewAssignment.EngineerId == 0 || NewAssignment.TaskId == 0)
                return Redirect(Request.Path + Request.QueryString);

            // carregar dados necessários para validação e re-render
            Engineers = await _api.GetEngineersAsync();
            Assignments = await _api.GetAssignmentsForTaskAsync(NewAssignment.TaskId) ?? new List<EngineerAssignment>();

            // garantir Task carregada para validar MaxResources
            if (Task == null || Task.TaskId == 0 || Task.TaskId != NewAssignment.TaskId)
            {
                Task = await _api.GetTaskAsync(NewAssignment.TaskId) ?? new DevCapacityWebApp.Models.Tasks();
            }

            // validação: não permitir exceder MaxResources (assume 0 => ilimitado)
            if (Task.MaxResources > 0 && Assignments.Count >= Task.MaxResources)
            {
                ModelState.AddModelError(string.Empty, $"Não é possível adicionar: número máximo de assignments atingido para esta task (MaxResources = {Task.MaxResources}).");
                // repovoar selects/assignments para re-render da página com a mensagem
                Initiatives = await _api.GetInitiativesAsync();
                Statuses = await _api.GetStatusesAsync();
                // Engineers e Assignments já carregados acima
                return Page();
            }

            var exists = Assignments.Exists(a => a.EngineerId == NewAssignment.EngineerId);
            if (exists)
            {
                ModelState.AddModelError("NewAssignment.EngineerId", "Este engineer já está atribuído a esta task.");

                // garantir que Task e selects estão preenchidos para re-render da página com erro
                Task = await _api.GetTaskAsync(NewAssignment.TaskId) ?? new DevCapacityWebApp.Models.Tasks();
                Initiatives = await _api.GetInitiativesAsync();
                Statuses = await _api.GetStatusesAsync();

                return Page();
            }

            // Defaults para campos readonly esperados pela API
            NewAssignment.CapacityShare = NewAssignment.CapacityShare == 0 ? 0 : NewAssignment.CapacityShare;
            if (NewAssignment.StartDate == default) NewAssignment.StartDate = DateTime.Today;
            if (NewAssignment.EndDate == default) NewAssignment.EndDate = DateTime.Today;

            await _api.CreateAssignmentAsync(NewAssignment);
            return RedirectToPage(new { id = NewAssignment.TaskId });
        }

        public async Task<IActionResult> OnPostDeleteAssignmentAsync(int id, int taskId)
        {
            await _api.DeleteAssignmentAsync(id);
            // redireciona explicitamente para a página de edição da task com o taskId correto
            return RedirectToPage(new { id = taskId });
        }
    }
}