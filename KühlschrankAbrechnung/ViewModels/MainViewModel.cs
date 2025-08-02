using Shared.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using DynamicData;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Timers;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media.Imaging;
using System.Xml.Linq;
using Avalonia.Controls;
using System.Reactive;
using Avalonia.Media.TextFormatting.Unicode;
using System.Threading;
namespace KühlschrankAbrechnung.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentView;
        private readonly System.Timers.Timer _timerSL;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ViewModelBase CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        private bool _showLockScreen = false;
        public bool ShowLockScreen
        {
            get => _showLockScreen;
            set => this.RaiseAndSetIfChanged(ref _showLockScreen, value);
        }

        private SatzViewModel SatzVM { get; }
        private HomeViewModel HomeVM { get; }
        private ErrorViewModel ErrorVM { get; }
        private KantinörViewModel KantinörVM { get; }
        private MusicianViewModel MusicianVM { get; set; }

        public MainViewModel()
        {
            HomeVM = new HomeViewModel(this);
            SatzVM = new SatzViewModel(this);
            ErrorVM = new ErrorViewModel(this);
            KantinörVM = new KantinörViewModel(this);

            CurrentView = HomeVM;

            InitMusicians();
            InitDrinks();
            InitKassa();

            // Timer für ScreenLock
            _timerSL = new System.Timers.Timer(600000); // In 10min
            _timerSL.Elapsed += (e, s) => SetShowLockScreen();
            _timerSL.AutoReset = false; // Nur einmal auslösen

            ResetLockScreenTimer();
        }

        public void CancelDrink(string musicianName, string drinkName)
        {
            DrinkEntryDB drinkToRemove;
            using (var db = new MusicianDbContext())
            {
                var musician = db.Musicians
                                 .Include(m => m.DrinkEntries)
                                 .FirstOrDefault(m => m.Name == musicianName);

                if (musician == null)
                    return;

                drinkToRemove = musician.DrinkEntries.LastOrDefault(d => d.Name == drinkName);
                if (drinkToRemove != null)
                {
                    musician.DrinkEntries.Remove(drinkToRemove);
                    musician.outBalance -= drinkToRemove.Price;
                    db.SaveChanges();

                    using (var drinksDb = new DrinksDbContext())
                    {
                        var drink = drinksDb.Drink.FirstOrDefault(d => d.Name == drinkName);
                        if (drink != null)
                        {
                            drink.StockCount++;
                            drinksDb.SaveChanges();
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            using (var db = new KassaDbContext())
            {
                var kassa = db.Kassa.FirstOrDefault();
                if (kassa != null)
                {
                    kassa.ProjectedBalance -= drinkToRemove.Price;
                    db.SaveChanges();
                }
            }

            GoToMusicianView(MusicianVM.ChosenMusician);
        }

        private void SetShowLockScreen()
        {
            ShowLockScreen = true;
        }

        public void UserInteracted()
        {
            ShowLockScreen = false;
            ResetLockScreenTimer();
        }

        public void ResetLockScreenTimer()
        {
            _timerSL.Stop();
            _timerSL.Start();
        }    

        private void InitMusicians()
        {
            using (var db = new MusicianDbContext())
            {
                if(db.Database.EnsureCreated())
                {
                    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "musicians.json");
                    var json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    var musicians = JsonSerializer.Deserialize<List<Musician>>(json, options);
                    var sortedMusicians = musicians.OrderBy(m => m.Name)
                                                   .ToList();

                    db.AddRange(sortedMusicians);
                    db.SaveChanges();
                }
            }
        }

        private void InitDrinks()
        {
            using (var db = new DrinksDbContext())
            {
                db.Database.EnsureCreated();    
            }
        }

        private void InitKassa()
        {
            using (var db = new KassaDbContext())
            {
                db.Database.EnsureCreated();
                var kassa = db.Kassa.FirstOrDefault();
                if (kassa == null)
                {
                    db.Add(new KassaDB(0.0, 0.0));
                    db.SaveChanges();
                }
            }
        }

        public void GoToSatzView(Satz satz)
        {
            SatzVM.ChosenSatz = satz;
            if (satz == Satz.Kantinör)
            {
                CurrentView = KantinörVM;
            }
            else
            {
                using (var db = new MusicianDbContext())
                {
                    var musicians = db.Musicians.Where(m => m.Instrument == satz).AsNoTracking().ToList();
                    SatzVM.Musicians = new ObservableCollection<Musician>(musicians.OrderBy(m=>m.Name));
                }
                CurrentView = SatzVM;
            }
        }

        public void GoToHomeView()
        {
            if(CurrentView is MusicianViewModel tempVM && tempVM.ChosenDrink != null)
            {
                    HomeVM.MusicianName = tempVM.ChosenMusician.Name;
                    HomeVM.DrinkName = tempVM.ChosenDrink.Name;
                    HomeVM.ShowDrinkPopup = true;
            }
            else
                HomeVM.ShowDrinkPopup = false;

            CurrentView = HomeVM;
        }

        public void GoToMusicianView(Musician musician)
        {
            var drinks = new List<DrinkDB>();
            using (var db = new DrinksDbContext())
            {
                drinks = db.Drink.AsNoTracking().Where(d => d.IsListed).ToList();
                foreach(var drink in drinks)
                {
                    drink.SetImagePath();
                }
            }

            MusicianVM = new MusicianViewModel(this);
            MusicianVM.Drinks = new ObservableCollection<DrinkDB>(drinks);
            MusicianVM.ChosenMusician = musician;
            CurrentView = MusicianVM;
        }

        public void GoToErrorView()
        {
            CurrentView = ErrorVM;
        }

        public void GoToKassaView()
        {
            CurrentView = new KassaViewModel(this, GetNewKassa());
        }

        public void GoToKantinörView()
        {
           //Unlock when its secured
            // CurrentView = KantinörVM;
        }

        public void AddKassaDebt(double amount)
        {
            using (var db = new KassaDbContext())
            {
                var kassa = db.Kassa.FirstOrDefault();
                if (kassa != null)
                {
                    kassa.ProjectedBalance += amount;
                    db.SaveChanges();
                }
            }
        }

        public void GoToAbrechnungsView()
        {
            var openDebtMusicians = new ObservableCollection<Musician>();
            using (var db = new MusicianDbContext())
            {
                openDebtMusicians = new ObservableCollection<Musician>(db.Musicians.Where(m => m.outBalance > 0.0)
                    .AsNoTracking()
                    .ToList());
            }

            CurrentView = new AbrechnungsViewModel(this, GetNewKassa(), openDebtMusicians);
        }

        private Kassa GetNewKassa()
        {
            Kassa kassa;
            using (var db = new KassaDbContext())
            {
                var dbKassa = db.Kassa.FirstOrDefault();
                kassa = new Kassa(dbKassa.Balance, dbKassa.ProjectedBalance);
            }
            return kassa;
        }

        public void GoToStatistikView()
        {
            var transactions = new List<TransactionDB>();
            var drinks = new List<DrinkEntryDB>();
            var musicians = new List<Musician>();
            using (var db = new KassaDbContext())
            {
                transactions = db.Transactions.Where(e => e.Date.Contains(DateTime.Now.Year.ToString())).AsNoTracking().ToList();
            }
            using (var db = new MusicianDbContext())
            {
                musicians = db.Musicians.AsNoTracking().ToList();
                drinks = db.DrinkEntries.Where(e => e.Date.Contains(DateTime.Now.Year.ToString())).AsNoTracking().ToList();
            }

            CurrentView = new StatistikViewModel(this, GetNewKassa(), transactions, drinks, musicians);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void PayDebt(Musician musician)
        {
            using (var db = new MusicianDbContext())
            {
                var updateMusician = db.Musicians.FirstOrDefault(m => m.Name == musician.Name);
                if (updateMusician == null) return;
                updateMusician.Balance += musician.outBalance;
                updateMusician.outBalance = 0.0;
                db.SaveChanges();
            }
            using (var db = new KassaDbContext())
            {
                var kassa = db.Kassa.FirstOrDefault();
                if (kassa != null)
                {
                    kassa.Balance += musician.outBalance;
                    kassa.Transactions.Add(new TransactionDB(
                            musician.outBalance,
                            $"Abrechnung {musician.Name}",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                            TransactionType.Einnahme
                        ));
                    db.SaveChanges();
                }
            }
        }
    }
}
