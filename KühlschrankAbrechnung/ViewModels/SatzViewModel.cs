using Shared.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace KühlschrankAbrechnung.ViewModels
{
    public partial class SatzViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;

        public Satz ChosenSatz { get; set; }
        public ObservableCollection<Musician> Musicians { get; set; }

        public ReactiveCommand<Unit, Unit> GoToHomeViewCommand { get; }
        public ReactiveCommand<Musician, Unit> GoToMusicianViewCommand { get; }

        public SatzViewModel(MainViewModel main)
        {
            _main = main;
            GoToHomeViewCommand = ReactiveCommand.Create(() => _main.GoToHomeView());
            GoToMusicianViewCommand = ReactiveCommand.Create<Musician>(musician =>
            {
                _main.GoToMusicianView(musician);
            });
        }
    }
}
