using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ManagementWeb.Components.ViewModels
{
    public class StatistikViewModel
    {
        public double KassaStand { get; set; }
        public double GesamtEinnahmen { get; set; }
        public double GesamtAusgaben { get; set; }
        public double GesamtLiter { get; set; }
        public double GesamtKosten { get; set; }

        public ObservableCollection<string> TopTrinker { get; } = new();
        public ObservableCollection<string> TopGetränke { get; } = new();

        public List<TransactionDB> KassaTransactions { get; set; }
        public List<DrinkEntryDB> DrinkEntries { get; set; }
        public List<DrinkDB> Drinks { get; set; }

        public StatistikViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
            LoadDashboard();
            LoadBest();
            LoadYearStatistiks();
        }

        private void LoadYearStatistiks()
        {

            using(var db = new KassaDbContext())
            {
                KassaTransactions = db.Transactions.ToList();
            }

            using(var db = new MusicianDbContext())
            {
                DrinkEntries = db.DrinkEntries.ToList();
            }

            using(var db = new DrinksDbContext())
            {
                Drinks = db.Drink.ToList();
            }


            //Alle Monate Hinzufuegen
            for(int i = 1; i<=12; i++)
            {
                KassaTransactions.Add(new TransactionDB(0, "OnlyForChart",$"1.{i}.{DateTime.Now.Year}", TransactionType.Einnahme));

                DrinkEntries.Add(new DrinkEntryDB(new DrinkDB("OnlyForChart", 0.0, 0.0, false), $"1.{i}.{DateTime.Now.Year}", new Musician("OnlzForChart", Satz.Tuba)));
            }

            KassaTransactions = KassaTransactions.OrderBy(e => DateTime.Parse(e.Date)).ToList();
            DrinkEntries = DrinkEntries.OrderBy(e => DateTime.Parse(e.Date)).ToList();
        }


        private void LoadDashboard()
        {
            using (var db = new KassaDbContext())
            {
                KassaStand = db.Kassa.AsNoTracking().FirstOrDefault().Balance;
                GesamtEinnahmen = db.Transactions.AsNoTracking().Where(t => t.TransactionType == TransactionType.Einnahme).Sum(t => t.Amount);
                GesamtAusgaben = db.Transactions.AsNoTracking().Where(t => t.TransactionType == TransactionType.Ausgabe).Sum(t => t.Amount);

            }

            using (var db = new MusicianDbContext())
            { 
                GesamtLiter = db.DrinkEntries.AsNoTracking().Sum(g => g.Content);
            }
        }

        private void LoadBest()
        {
            TopTrinker.Clear();
            TopGetränke.Clear();

            using (var db = new MusicianDbContext())
            {
                var topDrinkers = db.DrinkEntries.AsNoTracking()
                .GroupBy(e => e.MusicianId)
                .Select(g => new
                {
                    Id = g.Key,
                    Liter = g.Sum(e => e.Content)
                })
                .OrderByDescending(x => x.Liter)
                .Take(3)
                .ToList();

                var topDrinks = db.DrinkEntries.AsNoTracking()
                .GroupBy(e => e.Name)
                .Select(g => new
                {
                    Getränk = g.Key,
                    Anzahl = g.Count()
                })
                .OrderByDescending(x => x.Anzahl)
                .Take(3)
                .ToList();

                var musicians = db.Musicians.AsNoTracking().ToList();
                foreach (var person in topDrinkers)
                {
                    TopTrinker.Add($"{musicians.FirstOrDefault(m => m.Id == person.Id).Name} - {person.Liter:F1} L");
                }

                foreach (var drink in topDrinks)
                {
                    TopGetränke.Add($"{drink.Getränk} - {drink.Anzahl}x");
                }
            }
        }
    }
}
