using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DevCapacityWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace DevCapacityWebApp.Services
{
    public class AuthenticatedHttpMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthServiceApi _authService;

        public AuthenticatedHttpMessageHandler(IHttpContextAccessor httpContextAccessor, AuthServiceApi authService)
        {
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            var accessToken = context?.User?.FindFirst("access_token")?.Value;
            var refreshToken = context?.User?.FindFirst("refresh_token")?.Value;

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            // try refresh if we have a refresh token
            if (string.IsNullOrEmpty(refreshToken) || context == null)
                return response;

            var newTokens = await _authService.RefreshAsync(refreshToken);
            if (newTokens == null || string.IsNullOrEmpty(newTokens.AccessToken))
                return response;

            // rebuild claims: take existing claims except old tokens, then add new tokens
            var existing = context.User?.Claims ?? Enumerable.Empty<Claim>();
            var claims = new List<Claim>();
            foreach (var c in existing)
            {
                if (c.Type == "access_token" || c.Type == "refresh_token") continue;
                claims.Add(new Claim(c.Type, c.Value));
            }
            claims.Add(new Claim("access_token", newTokens.AccessToken));
            if (!string.IsNullOrEmpty(newTokens.RefreshToken))
                claims.Add(new Claim("refresh_token", newTokens.RefreshToken));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true
            });

            // retry the request with new token
            var cloned = await CloneHttpRequestMessageAsync(request);
            cloned.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);

            return await base.SendAsync(cloned, cancellationToken);
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // copy the request content (if any)
            if (req.Content != null)
            {
                var ms = await req.Content.ReadAsByteArrayAsync();
                var copiedContent = new ByteArrayContent(ms);
                // copy content headers
                foreach (var h in req.Content.Headers)
                    copiedContent.Headers.TryAddWithoutValidation(h.Key, h.Value);
                clone.Content = copiedContent;
            }

            // copy the request headers
            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            foreach (var prop in req.Options)
            {
                // copy options if needed (net runtime specific)
                try { clone.Options.Set(new HttpRequestOptionsKey<object>(prop.Key), prop.Value); } catch { }
            }

            return clone;
        }
    }
}
