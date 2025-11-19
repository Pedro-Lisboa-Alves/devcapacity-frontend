using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevCapacityWebApp.Models;

namespace DevCapacityWebApp.Services
{
    public class AuthServiceApi
    {
        private readonly HttpClient _http;
        public AuthServiceApi(HttpClient http) => _http = http;

        public async Task<AuthResponse?> LoginAsync(AuthRequest req)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(req);
                System.Console.WriteLine($"[AuthServiceApi] LoginAsync payload: {json}");
                var resp = await _http.PostAsJsonAsync("/v1/auth/login", req);
                var respText = await resp.Content.ReadAsStringAsync();
                System.Console.WriteLine($"[AuthServiceApi] LoginAsync response: {(int)resp.StatusCode} {resp.StatusCode} - {respText}");
                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<AuthResponse>();
            }
            catch (HttpRequestException ex)
            {
                var baseAddr = _http.BaseAddress?.ToString() ?? "(no base)";
                System.Console.WriteLine($"[AuthServiceApi] LoginAsync exception: {ex.Message}");
                throw new HttpRequestException($"AuthServiceApi.LoginAsync failed calling {baseAddr.TrimEnd('/')}/v1/auth/login: {ex.Message}", ex);
            }
        }

        public async Task<AuthResponse?> RefreshAsync(string refreshToken)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(new { refreshToken });
                System.Console.WriteLine($"[AuthServiceApi] RefreshAsync payload: {json}");
                var r = await _http.PostAsJsonAsync("/v1/auth/refresh", new { refreshToken });
                var respText = await r.Content.ReadAsStringAsync();
                System.Console.WriteLine($"[AuthServiceApi] RefreshAsync response: {(int)r.StatusCode} {r.StatusCode} - {respText}");
                if (!r.IsSuccessStatusCode) return null;
                return await r.Content.ReadFromJsonAsync<AuthResponse>();
            }
            catch (HttpRequestException ex)
            {
                var baseAddr = _http.BaseAddress?.ToString() ?? "(no base)";
                System.Console.WriteLine($"[AuthServiceApi] RefreshAsync exception: {ex.Message}");
                throw new HttpRequestException($"AuthServiceApi.RefreshAsync failed calling {baseAddr.TrimEnd('/')}/v1/auth/refresh: {ex.Message}", ex);
            }
        }
    }
}
