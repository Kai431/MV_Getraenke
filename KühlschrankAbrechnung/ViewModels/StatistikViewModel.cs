using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Data;
using ReactiveUI;
using System.Reactive;

namespace KühlschrankAbrechnung.ViewModels
{
    public class StatistikViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private readonly Kassa _kassa;
        private readonly List<TransactionDB> _transactions;
        private readonly List<DrinkEntryDB> _consumedDrinks;
        private readonly List<Musician> _musicians; 

        public double KassaStand { get; set; }
        public double GesamtEinnahmen { get; set; }
        public double GesamtAusgaben { get; set; }
        public double GesamtLiter { get; set; }
        public double GesamtKosten { get; set; }

        public ObservableCollection<string> TopTrinker { get; } = new();
        public ObservableCollection<string> TopGetränke { get; } = new();

        // Für LiveCharts (z. B. mit LiveChartsCore.SkiaSharpView.Avalonia)
        public ISeries[] LiterSeries { get; set; }
        public ISeries[] KassaSeries { get; set; }
        public Axis[] MonatsAxis { get; set; }
        public Axis[] WertAxisDrinks { get; set; }
        public Axis[] WertAxisKassa { get; set; }

        ReactiveCommand<Unit, Unit> GoBackCommand { get; }

        public StatistikViewModel(MainViewModel main, Kassa kassa, List<TransactionDB> transactions, List<DrinkEntryDB> consumedDrinks, List<Musician> musicians)
        {
            _main = main;
            _transactions = transactions;
            _consumedDrinks = consumedDrinks;
            _kassa = kassa;
            _musicians = musicians;

            GoBackCommand = ReactiveCommand.Create(() => _main.GoToKantinörView());

            LoadDashboard();
            LoadBestDrinkers();
            LoadBestDrinks();
            LoadYearStatistiks();
        }

        private void LoadDashboard()
        {
            KassaStand = _kassa.Balance;
            GesamtEinnahmen = _transactions.Where(t => t.TransactionType == TransactionType.Einnahme).Sum(t => t.Amount);
            GesamtAusgaben = _transactions.Where(t => t.TransactionType == TransactionType.Ausgabe).Sum(t => t.Amount);

            GesamtLiter = _consumedDrinks.Sum(g => g.Content);
            GesamtKosten = _consumedDrinks.Sum(g => g.Price);
        }

        private void LoadBestDrinkers()
        {
            var top = _consumedDrinks
                .GroupBy(e => e.MusicianId)
                .Select(g => new
                {
                    Id = g.Key,
                    Liter = g.Sum(e => e.Content)
                })
                .OrderByDescending(x => x.Liter)
                .Take(3)
                .ToList();

            TopTrinker.Clear();
            foreach (var person in top)
            {
                TopTrinker.Add($"{_musicians.FirstOrDefault(m => m.Id == person.Id).Name} - {person.Liter:F1} L");
            }
        }

        private void LoadBestDrinks()
        {
            var top = _consumedDrinks
                .GroupBy(e => e.Name)
                .Select(g => new
                {
                    Getränk = g.Key,
                    Anzahl = g.Count()
                })
                .OrderByDescending(x => x.Anzahl)
                .Take(3)
                .ToList();

            TopGetränke.Clear();
            foreach (var g in top)
            {
                TopGetränke.Add($"{g.Getränk} - {g.Anzahl}x");
            }
        }


        private void LoadYearStatistiks()
        {
            var alleMonate = Enumerable.Range(1, 12)
                .Select(m => new DateTime(DateTime.Now.Year, m, 1))
                .ToList();

            // 1. Liter-Daten vorbereiten
            var monatlich = _consumedDrinks
                .GroupBy(e => new { Jahr = DateTime.Parse(e.Date).Year, Monat = DateTime.Parse(e.Date).Month })
                .ToDictionary(
                    g => new DateTime(g.Key.Jahr, g.Key.Monat, 1).ToString("MMM"),
                    g => g.Sum(x => x.Content)
                );

            var literDaten = alleMonate
                .Select(d => monatlich.TryGetValue(d.ToString("MMM"), out var liter) ? liter : 0)
                .ToArray();

            // 2. Einnahmen/Ausgaben vorbereiten
            var einAus = _transactions
                .GroupBy(t => new { Jahr = DateTime.Parse(t.Date).Year, Monat = DateTime.Parse(t.Date).Month })
                .ToDictionary(
                    g => new DateTime(g.Key.Jahr, g.Key.Monat, 1).ToString("MMM"),
                    g => new
                    {
                        Einnahmen = g.Where(x => x.TransactionType == TransactionType.Einnahme).Sum(x => x.Amount),
                        Ausgaben = g.Where(x => x.TransactionType == TransactionType.Ausgabe).Sum(x => x.Amount)
                    }
                );

            var einnahmenDaten = alleMonate
                .Select(d => einAus.TryGetValue(d.ToString("MMM"), out var val) ? val.Einnahmen : 0)
                .ToArray();

            var ausgabenDaten = alleMonate
                .Select(d => einAus.TryGetValue(d.ToString("MMM"), out var val) ? val.Ausgaben : 0)
                .ToArray();

            var labels = alleMonate.Select(d => d.ToString("MMM")).ToArray();

            // 3. Liter-Diagramm
            LiterSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = literDaten,
                    Name = "Liter",
                }
            };

            // 4. Kassa-Diagramm
            KassaSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = einnahmenDaten,
                    Name = "Einnahmen"
                },
                new ColumnSeries<double>
                {
                    Values = ausgabenDaten,
                    Name = "Ausgaben"
                }
            };

            // 5. Achsen aktualisieren
            MonatsAxis = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = 0,
                    LabelsAlignment = LiveChartsCore.Drawing.Align.Start,
                    UnitWidth = 1, // sorgt dafür, dass die Balken genau einer "Einheit" entsprechen
                }
            };

            WertAxisDrinks = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0
                }
            };

            WertAxisKassa = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0
                }
            };
        }

    }
}
