using ManagementWeb.Components.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared.Data;
using Shared.Models;

namespace ManagementWeb.Components.ViewModels
{
    public class KassaTransaktionenViewModel
    {
        public TransactionType TransactionType { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? TransactionDate { get; set; } = DateTime.Today;
        public List<DashboardCard> DashboardCards { get; set; }
        public IQueryable<TransactionDB> Transactions { get; set; }

        private int dataYear = DateTime.Now.Year;
        public int DataYear
        {
            get { return dataYear; }
            set
            {
                dataYear = value;
                Transactions = GetTransactions(dataYear.ToString()).AsQueryable();
            }
        }


        private IMessageService _messageService;

        private double KassaStand { get; set; }
        private double GesamtEinnahmen { get; set; }
        private double GesamtAusgaben { get; set; }
        private double ProjectedBalance { get; set; }


        public KassaTransaktionenViewModel(IMessageService messageService)
        {
            _messageService = messageService;
            Initialize();
        }

        public void Initialize()
        {
            Transactions = GetTransactions(DataYear.ToString()).AsQueryable();

            DashboardCards = new List<DashboardCard>
            {
                new DashboardCard("Kassastand", $"€ {KassaStand:N2}", "mdi:cash-check", "#E3F2FD", "#0D47A1"),
                new DashboardCard("Prognostizierter Kassastand", $"€ {ProjectedBalance:N2}", "mdi:cash-clock", "#FFF3E0", "#EF6C00"),
                new DashboardCard("Einnahmen", $"€ {GesamtEinnahmen:N2}", "mdi:cash-plus", "#E8F5E9", "#2E7D32"),
                new DashboardCard("Ausgaben", $"€ {GesamtAusgaben:N2}", "mdi:cash-minus", "#FFEBEE", "#D32F2F"),
            };
        }

        private void ShowTopMessage(string message, MessageIntent type)
        {
            _messageService.ShowMessageBar(options =>
            {
                options.Title = message;
                options.Intent = type;
                options.Timeout = 4000;
            });
        }

        private List<TransactionDB> GetTransactions(string year)
        {
            using (var db = new KassaDbContext())
            {
                var kassaDb = db.Kassa.FirstOrDefault();
                KassaStand = kassaDb.Balance;
                ProjectedBalance = kassaDb.ProjectedBalance;
                GesamtEinnahmen = db.Transactions.Where(t => t.TransactionType == TransactionType.Einnahme).Sum(t => t.Amount);
                GesamtAusgaben = db.Transactions.Where(t => t.TransactionType == TransactionType.Ausgabe).Sum(t => t.Amount);

                return db.Transactions.AsNoTracking().Where(t => t.Date.Contains(year)).ToList();
            }
        }

        public void SaveTransaction()
        {
            if(Amount == 0.0 || Amount==null)
            {
                ShowTopMessage("Keine Transaktion gespeichert. Bitte Betrag ausfüllen!", MessageIntent.Error);
                return;
            }
            else if(Description == string.Empty)
            {
                ShowTopMessage("Keine Transaktion gespeichert. Bitte Beschreibung eingeben!", MessageIntent.Error);
                return;
            }
            else if (TransactionDate == null)
            {
                ShowTopMessage("Keine Transaktion gespeichert. Bitte Datum eingeben!", MessageIntent.Error);
                return;
            }

            var trans = new TransactionDB((double)Amount, Description, TransactionDate?.ToString("dd.MM.yyyy"), TransactionType);
            using (var db = new KassaDbContext())
            {
                var kassa = db.Kassa.FirstOrDefault();
                if (trans.TransactionType == TransactionType.Einnahme)
                {
                    kassa.Balance += trans.Amount;
                    kassa.ProjectedBalance += trans.Amount;
                }
                else
                {
                    kassa.Balance -= trans.Amount;
                    kassa.ProjectedBalance -= trans.Amount;
                }

                kassa.Transactions.Add(trans);
                db.SaveChanges();
            }

            Initialize();
        }
    }
}
