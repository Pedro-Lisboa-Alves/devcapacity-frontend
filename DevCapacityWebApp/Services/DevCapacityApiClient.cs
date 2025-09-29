using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevCapacityWebApp.Models;

namespace DevCapacityWebApp.Services
{
    public class DevCapacityApiClient
    {
        private readonly HttpClient _http;
        public DevCapacityApiClient(HttpClient http) => _http = http;

        // Engineers
        public async Task<List<Engineer>> GetEngineersAsync()
        {
            var list = await _http.GetFromJsonAsync<List<Engineer>>("/engineers");
            return list ?? new List<Engineer>();
        }

        public Task<Engineer?> GetEngineerAsync(int id) =>
            _http.GetFromJsonAsync<Engineer>($"/engineers/{id}");

        public async Task<bool> CreateEngineerAsync(Engineer e)
        {
            var r = await _http.PostAsJsonAsync("/engineers", e);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateEngineerAsync(int id, Engineer e)
        {
            var r = await _http.PutAsJsonAsync($"/engineers/{id}", e);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEngineerAsync(int id)
        {
            var r = await _http.DeleteAsync($"/engineers/{id}");
            return r.IsSuccessStatusCode;
        }

        // Teams (corrigido para plural /teams)
        public async Task<List<Team>> GetTeamsAsync()
        {
            var list = await _http.GetFromJsonAsync<List<Team>>("/team");
            return list ?? new List<Team>();
        }

        public Task<Team?> GetTeamAsync(int id) =>
            _http.GetFromJsonAsync<Team>($"/team/{id}");

        public async Task<bool> CreateTeamAsync(Team t)
        {
            var r = await _http.PostAsJsonAsync("/team", t);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTeamAsync(int id, Team t)
        {
            var r = await _http.PutAsJsonAsync($"/team/{id}", t);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var r = await _http.DeleteAsync($"/team/{id}");
            return r.IsSuccessStatusCode;
        }

        // Tasks
        public async Task<List<TaskItem>> GetTasksAsync()
        {
            var list = await _http.GetFromJsonAsync<List<TaskItem>>("/tasks");
            return list ?? new List<TaskItem>();
        }

        public Task<TaskItem?> GetTaskAsync(int id) =>
            _http.GetFromJsonAsync<TaskItem>($"/tasks/{id}");

        public async Task<bool> CreateTaskAsync(TaskItem t)
        {
            var r = await _http.PostAsJsonAsync("/tasks", t);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTaskAsync(int id, TaskItem t)
        {
            var r = await _http.PutAsJsonAsync($"/tasks/{id}", t);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var r = await _http.DeleteAsync($"/tasks/{id}");
            return r.IsSuccessStatusCode;
        }

        // Initiatives
        public async Task<List<Initiatives>> GetInitiativesAsync()
        {
            var list = await _http.GetFromJsonAsync<List<Initiatives>>("/initiatives");
            return list ?? new List<Initiatives>();
        }

        public Task<Initiatives?> GetInitiativeAsync(int id) =>
            _http.GetFromJsonAsync<Initiatives>($"/initiatives/{id}");

        public async Task<bool> CreateInitiativeAsync(Initiatives i)
        {
            var r = await _http.PostAsJsonAsync("/initiatives", i);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateInitiativeAsync(int id, Initiatives i)
        {
            var r = await _http.PutAsJsonAsync($"/initiatives/{id}", i);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInitiativeAsync(int id)
        {
            var r = await _http.DeleteAsync($"/initiatives/{id}");
            return r.IsSuccessStatusCode;
        }

        // Engineer Assignments (for Tasks)
        public async Task<List<EngineerAssignment>> GetAssignmentsForTaskAsync(int taskId)
        {
            var list = await _http.GetFromJsonAsync<List<EngineerAssignment>>($"/tasks/{taskId}/assignments");
            return list ?? new List<EngineerAssignment>();
        }

        public async Task<bool> CreateAssignmentAsync(EngineerAssignment a)
        {
            var r = await _http.PostAsJsonAsync("/assignments", a);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var r = await _http.DeleteAsync($"/assignments/{id}");
            return r.IsSuccessStatusCode;
        }

        // Statuses
        public async Task<List<Status>> GetStatusesAsync()
        {
            var list = await _http.GetFromJsonAsync<List<Status>>("/status");
            return list ?? new List<Status>();
        }
    }
}