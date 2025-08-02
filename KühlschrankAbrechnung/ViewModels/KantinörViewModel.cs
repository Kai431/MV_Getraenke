using Shared.Data;
using Shared.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KühlschrankAbrechnung.ViewModels
{
    public class KantinörViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        public ReactiveCommand<Unit, Unit> GoToHomeViewCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToKassaViewCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToAbrechnungsViewCommand { get; }
        public ReactiveCommand<Unit, Unit> GoToStatistikViewCommand { get; }

        public KantinörViewModel(MainViewModel main)
        {
            _main = main;
            GoToHomeViewCommand = ReactiveCommand.Create(()=> _main.GoToHomeView());
            GoToKassaViewCommand = ReactiveCommand.Create(() => _main.GoToKassaView());
            GoToAbrechnungsViewCommand = ReactiveCommand.Create(() => _main.GoToAbrechnungsView());
            GoToStatistikViewCommand = ReactiveCommand.Create(() => _main.GoToStatistikView());
        }
    }
}
