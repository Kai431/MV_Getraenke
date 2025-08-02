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
    public class KassaViewModel : ViewModelBase
    {
        private readonly Kassa _kassa;
        private readonly MainViewModel _main;

        public ReactiveCommand<Unit, Unit> GoBackCommand { get; }
        public KassaViewModel(MainViewModel main, Kassa kassa)
        {
            _main = main;
            _kassa = kassa;

            GoBackCommand = ReactiveCommand.Create(() => _main.GoToKantinörView());
        }

        public string BalanceFormatted => $"{_kassa.Balance:0.00} €";
        public string ProjectedFormatted => $"{_kassa.ProjectedBalance:0.00} €";

        // Optional: Für DataBinding an ProgressBars o. Ä.
        public double Balance => _kassa.Balance;
        public double Projected => _kassa.ProjectedBalance;
    }
}
