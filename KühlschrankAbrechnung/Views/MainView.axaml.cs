using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using KühlschrankAbrechnung.ViewModels;
using System;
using System.Threading;

namespace KühlschrankAbrechnung;

public partial class MainView : Window
{
    private readonly Timer _lockScreenTimer;

    public MainView()
    {
        InitializeComponent();

        this.AddHandler(InputElement.PointerPressedEvent, OnAnyPointer, RoutingStrategies.Tunnel, true);
        //this.AddHandler(InputElement.PointerMovedEvent, OnAnyPointer, RoutingStrategies.Tunnel, true);
    }

    private void OnAnyPointer(object? sender, PointerEventArgs e)
    {
        if (DataContext is MainViewModel mainVM)
        {
            mainVM.UserInteracted(); // <-- Deine Methode
        }

    }
}