using Avalonia.Threading;
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
using System.Timers;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using System.Xml.Linq;
using System.IO;

namespace KühlschrankAbrechnung.ViewModels
{
    public class HomeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly MainViewModel _main;

        private readonly Timer _timer;

        private string _date;
        public string Date
        {
            get => _date;
            private set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _musicianName = "Kai";
        public string MusicianName
        {
            get => _musicianName;
            set
            {
                if (_musicianName != value)
                {
                    _musicianName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _drinkName = "Limo";
        public string DrinkName
        {
            get => _drinkName;
            set
            {
                if (_drinkName != value)
                {
                    _drinkName = value;
                    ImageName = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"Assets/Icons/Drinks/{_drinkName}.png"));
                    OnPropertyChanged();
                }
            }
        }

        private Bitmap _imageName = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"Assets/Icons/Drinks/Limo.png"));
        public Bitmap ImageName
        {
            get => _imageName;
            private set
            {
                _imageName = value;
                OnPropertyChanged();
            }
        }

        public ReactiveCommand<Unit, Unit> CancelDrinkCommand { get; }

        private bool _showDrinkPopup = false;
        public bool ShowDrinkPopup
        {
            get => _showDrinkPopup;
            set
            {
                if (_showDrinkPopup != value)
                {
                    _showDrinkPopup = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactiveCommand<string, Unit> GoToSatzViewCommand { get; }

        public HomeViewModel(MainViewModel main)
        {
            _main = main;
            GoToSatzViewCommand = ReactiveCommand.Create<string>(satzName =>
            {
                Satz satz = Enum.Parse<Satz>(satzName);
                if (satz == Satz.Kantinör)
                    _main.GoToKantinörView();
                else
                    _main.GoToSatzView(satz);
            });

            UpdateDate();
            CancelDrinkCommand = ReactiveCommand.Create(() => _main.CancelDrink(MusicianName, DrinkName));

            // Timer für Datum und Uhrzeit
            _timer = new Timer(60000); // Alle 60 Sekunden
            _timer.Elapsed += (s, e) => UpdateDate();
            _timer.Start();
        }

        private void UpdateDate()
        {
            Date = DateTime.Now.ToString("dd.MM.yyyy – HH:mm", new CultureInfo("de-DE"));
        }


        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
