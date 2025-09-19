var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register DevCapacity API client (ajuste BaseAddress para sua API)
// NOTE: só usar AddHttpClient<TClient>(); não registar AddScoped novamente.
builder.Services.AddHttpClient<DevCapacityWebApp.Services.DevCapacityApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5144/");
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

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

