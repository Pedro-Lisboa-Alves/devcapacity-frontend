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
        public DevCapacityWebApp.Models.Tasks Task { get; set; } = new();

        public List<DevCapacityWebApp.Models.Initiatives> Initiatives { get; set; } = new();
        public List<DevCapacityWebApp.Models.Status> Statuses { get; set; } = new();
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
            if (NewAssignment == null)
                return Redirect(Request.Path + Request.QueryString);

            // carrega engineers (usado para heurística de binding)
            Engineers = await _api.GetEngineersAsync();

            // --- TaskId fallback ---
            if (NewAssignment.TaskId == 0)
            {
                // tentativas explícitas
                var formTask = Request.Form["NewAssignment.TaskId"].ToString();
                if (!string.IsNullOrEmpty(formTask) && int.TryParse(formTask, out var fid))
                {
                    NewAssignment.TaskId = fid;
                }
                else
                {
                    var alt = Request.Form["TaskId"].ToString();
                    if (!string.IsNullOrEmpty(alt) && int.TryParse(alt, out fid))
                    {
                        NewAssignment.TaskId = fid;
                    }
                    else
                    {
                        var qs = Request.Query["id"].ToString();
                        if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out var qid))
                        {
                            NewAssignment.TaskId = qid;
                        }
                        else if (RouteData.Values.TryGetValue("id", out var ridObj) && int.TryParse(ridObj?.ToString(), out var rid))
                        {
                            NewAssignment.TaskId = rid;
                        }
                    }
                }
            }

            // --- EngineerId fallback ---
            if (NewAssignment.EngineerId == 0)
            {
                // tentativas explícitas por nomes comuns
                var formEng = Request.Form["NewAssignment.EngineerId"].ToString();
                if (!string.IsNullOrEmpty(formEng) && int.TryParse(formEng, out var eid))
                {
                    NewAssignment.EngineerId = eid;
                }
                else
                {
                    var altEng = Request.Form["EngineerId"].ToString();
                    if (!string.IsNullOrEmpty(altEng) && int.TryParse(altEng, out eid))
                    {
                        NewAssignment.EngineerId = eid;
                    }
                    else
                    {
                        // heurística: percorre todos os valores do form e tenta encontrar um que corresponda ao id de um engineer conhecido
                        foreach (var kv in Request.Form)
                        {
                            foreach (var val in kv.Value)
                            {
                                if (int.TryParse(val, out var possible) && Engineers != null && Engineers.Exists(x => x.EngineerId == possible))
                                {
                                    NewAssignment.EngineerId = possible;
                                    break;
                                }
                            }
                            if (NewAssignment.EngineerId != 0) break;
                        }
                    }
                }
            }

            // valida obrigatoriedade mínima
            if (NewAssignment.EngineerId == 0 || NewAssignment.TaskId == 0)
            {
                return Redirect(Request.Path + Request.QueryString);
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