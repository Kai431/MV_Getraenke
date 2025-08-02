using Avalonia.Controls;
using Shared.Data;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KühlschrankAbrechnung.ViewModels
{
    public class MusicianViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;

        public UserControl DrinksView { get; }
        public UserControl MusicianInfoView { get; }

        private UserControl _currentMusicianContent;
        public UserControl CurrentMusicianContent
        {
            get => _currentMusicianContent;
            set => this.RaiseAndSetIfChanged(ref _currentMusicianContent, value);
        }


        public DrinkDB ChosenDrink;

        private Musician _chosenMusician;
        public Musician ChosenMusician
        {
            get { return _chosenMusician; }
            set
            {
                if (value != null)
                {
                    Musician musician;
                    using (var db = new MusicianDbContext())
                    {
                        musician = db.Musicians
                                         .Include(m => m.DrinkEntries)
                                         .FirstOrDefault(m => m.Name == value.Name);
                    }
                    if (musician != null)
                    {
                        MakeDataForDataGrid(musician.DrinkEntries.ToList());
                    }
                    _chosenMusician = value;
                }
            }
        }

        public ObservableCollection<DrinkDB> Drinks { get; set; } = new ObservableCollection<DrinkDB>();
        public ObservableCollection<DataGridDrink> DataGridItems { get; set; } = new ObservableCollection<DataGridDrink>();

        public ReactiveCommand<Unit, Unit> GoToHomeViewCommand { get; }
        public ReactiveCommand<DrinkDB, Unit> AddDrinkCommand { get; }
        public ReactiveCommand<Unit, Unit> SwitchUserControleCommand { get; }

        public MusicianViewModel(MainViewModel main)
        {
            _main = main;
            DrinksView = new DrinksView();
            MusicianInfoView = new MusicianInfoView();

            CurrentMusicianContent = DrinksView;

            GoToHomeViewCommand = ReactiveCommand.Create(() => _main.GoToHomeView());
            AddDrinkCommand = ReactiveCommand.Create<DrinkDB>(AddDrink);
            SwitchUserControleCommand = ReactiveCommand.Create(() =>
            {
                if (CurrentMusicianContent is DrinksView)
                {
                    CurrentMusicianContent = MusicianInfoView;
                }
                else
                {
                    CurrentMusicianContent = DrinksView;
                }
            });

        }

        public void AddDrink(DrinkDB drink)
        {
            ChosenDrink = drink;
            using (var db = new MusicianDbContext())
            {
                var musician = db.Musicians.FirstOrDefault(m => m.Name == ChosenMusician.Name);
                if (musician != null)
                {
                    musician.outBalance += drink.Price;
                    musician.DrinkEntries.Add(new DrinkEntryDB(drink, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), musician));
                    db.SaveChanges();
                }
            }

            using (var db = new DrinksDbContext())
            {
                db.Drink.FirstOrDefault(d => d.Name == drink.Name).StockCount--;
                db.SaveChanges();
            }

            _main.AddKassaDebt(drink.Price);

            _main.GoToHomeView();
        }

        public void MakeDataForDataGrid(List<DrinkEntryDB> drinkEntries)
        {
            var consumed = drinkEntries
                           .GroupBy(d => d.Name)
                           .ToDictionary(
                               g => g.Key,
                               g => new
                               {
                                   Count = g.Count(),
                                   ContentSum = g.Sum(d => d.Content),
                                   CostSum = g.Sum(d => d.Price)
                               });

            var summary = Drinks.Select(drink =>
            {
                var name = drink.Name;
                if (consumed.TryGetValue(name, out var stats))
                {
                    return new DataGridDrink(drink, stats.Count, stats.ContentSum, stats.CostSum);
                }
                else
                {
                    return new DataGridDrink(drink, 0, 0.0, 0.0);
                }
            });

            DataGridItems = new ObservableCollection<DataGridDrink>(summary);
        }
    }
}
