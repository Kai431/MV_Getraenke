using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using KühlschrankAbrechnung.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using ManagementWeb;
using System.Diagnostics;
using System;
using System.IO;
using Avalonia.Controls;
using Serilog;

namespace KühlschrankAbrechnung;

public partial class App : Application
{
    public override void Initialize()
    {
        // Serilog Setup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File("logs/getränkeApp-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Globale Fehler abfangen
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");
            Log.CloseAndFlush();
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Log.Fatal(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
            Log.CloseAndFlush();
        };

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            Log.Information("GetränkeApp starten...");
            desktop.MainWindow = new MainView
            {
                //WindowState = WindowState.FullScreen,
                DataContext = new MainViewModel(),
            };
        }

        var webServer = new WebServerManager();
        webServer.StartServer();
        Log.Information("Webserver gestartet");

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}