using Shared.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KühlschrankAbrechnung.ViewModels
{
    public class ErrorViewModel : ViewModelBase
    {

        private readonly MainViewModel _main;
        public ReactiveCommand<Unit, Unit> GoToHomeViewCommand { get; }
        public ErrorViewModel(MainViewModel main)
        {
            _main = main;
            GoToHomeViewCommand = ReactiveCommand.Create(() => _main.GoToHomeView());
        }
    }
}
