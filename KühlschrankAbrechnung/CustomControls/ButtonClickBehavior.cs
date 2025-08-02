using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;

namespace KühlschrankAbrechnung
{
    public class ButtonClickBehavior : AvaloniaObject
    {
        static ButtonClickBehavior()
        {
            CommandProperty.Changed
                .AddClassHandler<Button>(OnCommandChanged);
        }

        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<ButtonClickBehavior, Button, ICommand?>(
                "Command", default, false, BindingMode.OneWay);

        public static readonly AttachedProperty<object?> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<ButtonClickBehavior, Button, object?>(
                "CommandParameter", default, false, BindingMode.OneWay);

        private static void OnCommandChanged(Button button, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is ICommand)
                button.Click += Button_Click;
            else
                button.Click -= Button_Click;
        }

        private static void Button_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var cmd = btn.GetValue(CommandProperty);
                var param = btn.GetValue(CommandParameterProperty);
                if (cmd != null && cmd.CanExecute(param))
                    cmd.Execute(param);
            }
        }

        public static void SetCommand(Button element, ICommand? value)
            => element.SetValue(CommandProperty, value);

        public static ICommand? GetCommand(Button element)
            => element.GetValue(CommandProperty);

        public static void SetCommandParameter(Button element, object? value)
            => element.SetValue(CommandParameterProperty, value);

        public static object? GetCommandParameter(Button element)
            => element.GetValue(CommandParameterProperty);
    }

}
