using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DevCapacityWebApp.Models;
using DevCapacityWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DevCapacityWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthServiceApi _auth;
        public LoginModel(AuthServiceApi auth) => _auth = auth;

        [BindProperty]
        public AuthRequest Input { get; set; } = new AuthRequest();

        public string? ErrorMessage { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            // show page
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(Input.Email) || string.IsNullOrEmpty(Input.Password) || string.IsNullOrEmpty(Input.Model))
            {
                ErrorMessage = "Enter email, password and model";
                return Page();
            }

            var res = await _auth.LoginAsync(Input);
            if (res == null || string.IsNullOrEmpty(res.AccessToken))
            {
                ErrorMessage = "Invalid credentials";
                return Page();
            }

            var claims = new List<Claim>();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(res.AccessToken);
                foreach (var c in jwt.Claims)
                {
                    claims.Add(new Claim(c.Type, c.Value));
                }
            }
            catch
            {
                try
                {
                    var parts = res.AccessToken.Split('.');
                    if (parts.Length >= 2)
                    {
                        var payload = parts[1];
                        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                        var bytes = System.Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
                        var json = System.Text.Encoding.UTF8.GetString(bytes);
                        using var doc = JsonDocument.Parse(json);
                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            var name = prop.Name;
                            var val = prop.Value.ToString();
                            claims.Add(new Claim(name, val ?? string.Empty));
                        }
                    }
                }
                catch { }
            }

            claims.Add(new Claim("access_token", res.AccessToken));
            if (!string.IsNullOrEmpty(res.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", res.RefreshToken));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(res.ExpiresIn > 0 ? res.ExpiresIn : 3600)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToPage("/Index");
        }
    }
}
