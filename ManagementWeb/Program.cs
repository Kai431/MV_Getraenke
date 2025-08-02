using ApexCharts;
using ManagementWeb.Components;
using ManagementWeb.Components.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared.Data;

var opt = new WebApplicationOptions
{
    ApplicationName = "ManagementWeb",
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
};

var builder = WebApplication.CreateBuilder();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddApexCharts();

builder.Services.AddScoped<DashBoardViewModel>();
builder.Services.AddScoped<MusicianManagerViewModel>();
builder.Services.AddScoped<AbrechnungViewModel>();
builder.Services.AddScoped<KassaTransaktionenViewModel>();
builder.Services.AddScoped<DownloadsViewModel>();
builder.Services.AddScoped<StatistikViewModel>();
builder.Services.AddScoped<DrinksViewModel>();
builder.Services.AddFluentUIComponents();
builder.Services.AddControllers();
builder.Services.AddDbContextFactory<KassaDbContext>();
builder.Services.AddDbContextFactory<MusicianDbContext>();

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();
using (var context = app.Services.GetRequiredService<IDbContextFactory<KassaDbContext>>().CreateDbContext())
{
    context.Database.EnsureCreated();
}
using (var context = app.Services.GetRequiredService<IDbContextFactory<KassaDbContext>>().CreateDbContext())
{
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

//app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapControllers(); // ❗Wichtig

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run("http://0.0.0.0:5000");