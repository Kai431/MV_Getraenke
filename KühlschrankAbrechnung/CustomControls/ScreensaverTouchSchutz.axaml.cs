using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;

namespace KühlschrankAbrechnung;

public partial class ScreensaverTouchSchutz : UserControl
{
    public ScreensaverTouchSchutz()
    {
        InitializeComponent();
        this.PointerPressed += (_, __) => IsVisible = false;
    }
}