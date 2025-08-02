using ManagementWeb.Components.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace ManagementWeb.Components.ViewModels
{
    public class DashBoardViewModel
    {
        private double KassaStand { get; set; }
        private double GesamtEinnahmen { get; set; }
        private double GesamtAusgaben { get; set; }
        private double GesamtLiter { get; set; }
        private double GesamtKosten { get; set; }
        private double ProjectedBalance { get; set; }
        private List<TransactionDB> _transactions;
        private List<DrinkEntryDB> _consumedDrinks;

        public List<DashboardCard> DashboardCards { get; set; }
        private readonly IDbContextFactory<KassaDbContext> _kassaDbFactory;
        private readonly IDbContextFactory<MusicianDbContext> _musicianDbFactory;
        private Kassa _kassa { get; set; }
        public DashBoardViewModel(IDbContextFactory<KassaDbContext> kassaDbFactory, IDbContextFactory<MusicianDbContext> musicianDbFactory) 
        {
            _kassaDbFactory = kassaDbFactory;
            _musicianDbFactory = musicianDbFactory;

            Initialize();
        }

        public void Initialize()
        {
            using (var db = _kassaDbFactory.CreateDbContext())
            {
                var kassaDb = db.Kassa.FirstOrDefault();
                if(kassaDb == null)
                {
                    kassaDb = new KassaDB(0.0, 0.0);
                    db.Kassa.Add(kassaDb);
                    db.SaveChanges();
                }
                else
                    _kassa = new Kassa(kassaDb.Balance, kassaDb.ProjectedBalance);
                _transactions = db.Transactions.ToList();
            }

            using (var db = _musicianDbFactory.CreateDbContext())
            {
                _consumedDrinks = db.DrinkEntries.AsNoTracking().ToList();
            }


            KassaStand = _kassa.Balance;
            ProjectedBalance = _kassa.ProjectedBalance;
            GesamtEinnahmen = _transactions.Where(t => t.TransactionType == TransactionType.Einnahme).Sum(t => t.Amount);
            GesamtAusgaben = _transactions.Where(t => t.TransactionType == TransactionType.Ausgabe).Sum(t => t.Amount);

            GesamtLiter = _consumedDrinks.Sum(g => g.Content);
            //GesamtKosten = _consumedDrinks.Sum(g => g.Price);

            DashboardCards = new List<DashboardCard>
            {
                new DashboardCard("Kassastand", $"€ {KassaStand:N2}", "mdi:cash-check", "#E3F2FD", "#0D47A1"),
                new DashboardCard("Prognostizierter Kassastand", $"€ {ProjectedBalance:N2}", "mdi:cash-clock", "#FFF3E0", "#EF6C00"),
                new DashboardCard("Einnahmen", $"€ {GesamtEinnahmen:N2}", "mdi:cash-plus", "#E8F5E9", "#2E7D32"),
                new DashboardCard("Ausgaben", $"€ {GesamtAusgaben:N2}", "mdi:cash-minus", "#FFEBEE", "#D32F2F"),
                new DashboardCard("Getränke (L)", $"L {GesamtLiter:N2}", "mdi:glass-mug-variant", "#E1F5FE", "#0288D1"),
                //new DashboardCard("Getränke (€)", $"€ {GesamtKosten:N2}", "mdi:receipt-text-minus-outline", "#F3E5F5", "#8E24AA"),
            };
        }
    }
}
