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
    }
}