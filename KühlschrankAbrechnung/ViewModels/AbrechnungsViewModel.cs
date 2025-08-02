using Shared.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KühlschrankAbrechnung.ViewModels
{
    public class AbrechnungsViewModel : ViewModelBase
    {
        public ReactiveCommand<Musician, Unit> BezahlenCommand { get; }
        public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

        private readonly Kassa _kassa;
        private readonly MainViewModel _main;
        public ObservableCollection<Musician> Musicians { get; private set; }

        public AbrechnungsViewModel(MainViewModel main,Kassa kassa, ObservableCollection<Musician> musicians)
        {
            _kassa = kassa;
            _main = main;
            Musicians = musicians;


            BezahlenCommand = ReactiveCommand.Create<Musician>(musician =>
            {
                _main.PayDebt(musician);
                Musicians.Remove(musician);
            });
            GoBackCommand = ReactiveCommand.Create(() => _main.GoToKantinörView());
        }

        
    }
}
