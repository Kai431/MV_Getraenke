using ApexCharts;
using ManagementWeb.Components.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Serilog;
using Shared.Data;
using System.Diagnostics;

namespace ManagementWeb
{
    public static class WebServerFactory
    {
        public static WebApplication CreateWebApp()
        {
            var opt = new WebApplicationOptions
            {
                ApplicationName = "ManagementWeb",
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
            };

            var builder = WebApplication.CreateBuilder(opt);

            // Serilog konfigurieren
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File("logs/webserver-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddHttpContextAccessor(); // falls du später Benutzer/IP loggen willst
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

            builder.WebHost.UseUrls("http://0.0.0.0:5000");

            if(builder.Environment.IsDevelopment())
                builder.WebHost.UseStaticWebAssets();

                var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers(); // ❗Wichtig
            app.UseAntiforgery();
            app.MapRazorComponents<ManagementWeb.Components.App>()
                .AddInteractiveServerRenderMode();

            return app;
        }
    }

}
