var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configuration for API base
var apiBase = builder.Configuration["DEV_CAPACITY_API_BASE"] ?? builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5144/";

// register IHttpContextAccessor for handlers that need the current user
builder.Services.AddHttpContextAccessor();

// register a message handler that attaches the current user's access token
builder.Services.AddTransient<DevCapacityWebApp.Services.AuthenticatedHttpMessageHandler>();

// Register DevCapacity API client with handler that adds Authorization from current user
builder.Services.AddHttpClient<DevCapacityWebApp.Services.DevCapacityApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBase);
}).AddHttpMessageHandler<DevCapacityWebApp.Services.AuthenticatedHttpMessageHandler>();

// Register AuthServiceApi to call authentication endpoints (separate base)
var authBase = builder.Configuration["AUTH_SERVICE_API_BASE"] ?? "https://localhost:7085/";
// If running in Development or using localhost, allow self-signed certs for the Auth client
if (builder.Environment.IsDevelopment() || authBase.StartsWith("https://localhost"))
{
    builder.Services.AddHttpClient<DevCapacityWebApp.Services.AuthServiceApi>(client =>
    {
        client.BaseAddress = new Uri(authBase);
    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
}
else
{
    builder.Services.AddHttpClient<DevCapacityWebApp.Services.AuthServiceApi>(client =>
    {
        client.BaseAddress = new Uri(authBase);
    });
}

// Configure cookie authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

