using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KühlschrankAbrechnung;



public partial class AuswahlPopup : UserControl, INotifyPropertyChanged
{
    public AuswahlPopup()
    {
        InitializeComponent();
        this.PointerPressed += (_, __) => IsVisible = false;

        //this.GetObservable(IsVisibleProperty).Subscribe(visible =>
        //{
        //    if (visible)
        //    {
        //        _ = StartAutoClose(); // Fire and forget
        //    }
        //});
    }

    public static readonly StyledProperty<ICommand> ExecuteCommandProperty =
    AvaloniaProperty.Register<AuswahlPopup, ICommand>(nameof(ExecuteCommand));
    public ICommand ExecuteCommand
    {
        get => GetValue(ExecuteCommandProperty);
        set => SetValue(ExecuteCommandProperty, value);
    }

    public static readonly StyledProperty<string> MusikerNameProperty =
    AvaloniaProperty.Register<AuswahlPopup, string>(nameof(MusikerName));

    public string MusikerName
    {
        get => GetValue(MusikerNameProperty);
        set => SetValue(MusikerNameProperty, value);
    }

    public static readonly StyledProperty<string> GetränkProperty =
       AvaloniaProperty.Register<AuswahlPopup, string>(nameof(Getränk));

    public string Getränk
    {
        get => GetValue(GetränkProperty);
        set => SetValue(GetränkProperty, value);
    }


    public static readonly StyledProperty<Bitmap?> BildProperty =
     AvaloniaProperty.Register<AuswahlPopup, Bitmap?>(nameof(Bild));

    public Bitmap? Bild
    {
        get => GetValue(BildProperty);
        set => SetValue(BildProperty, value);
    }

    public static readonly StyledProperty<int> ShowTimeProperty =
     AvaloniaProperty.Register<AuswahlPopup, int>(nameof(ShowTime));

    public int ShowTime
    {
        get => GetValue(ShowTimeProperty);
        set => SetValue(ShowTimeProperty, value);
    }

    private bool _autoStarted = false;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Starte nur, wenn sichtbar und noch nicht gestartet
        if (IsVisible && !_autoStarted)
        {
            _autoStarted = true;

            // Starte mit etwas Verzögerung, damit Bindings durchlaufen sind
            Dispatcher.UIThread.Post(() =>
            {
                StartAutoClose();
            }, DispatcherPriority.Background);
        }
    }

    public async Task StartAutoClose()
    {
        if (Progress == null)
            return;

        Progress.Value = 0;
        int steps = 100;
        int delay = ShowTime / steps;

        for (int i = 0; i <= steps; i++)
        {
            Progress.Value = i;
            await Task.Delay(delay);
        }

        this.IsVisible = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

