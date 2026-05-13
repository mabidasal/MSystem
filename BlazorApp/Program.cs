using BlazorApp.Components;
using BlazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// .NET 10 Blazor Web App wiring
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register workload service
builder.Services.AddScoped<WorkloadDataService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
