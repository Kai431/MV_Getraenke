using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared.Data;
using Shared.Models;

namespace ManagementWeb.Components.ViewModels
{
    public class AbrechnungViewModel
    {
        public List<Musician> Musicians { get; set; } = new List<Musician>();
        public Musician SelectedMusician { get; set; }
        public double? PayAmount { get; set; } = 0.0;

        private readonly IMessageService _messageService;
        public AbrechnungViewModel( IMessageService messageService)
        {
            _messageService = messageService;
            Initialize();
        }

        public void Initialize()
        {
            using (var db = new MusicianDbContext())
            {
                Musicians = db.Musicians.AsNoTracking().Where<Musician>(m => m.outBalance > 0.0).OrderBy(m => m.Name).ToList();
            }
            SelectedMusician = null;
        }
        
        public void MusicianChanged(Musician musician)
        {
            if(musician != null)
                SelectedMusician = musician;

            PayAmount = SelectedMusician.outBalance;
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

        public void Pay()
        {
            if (SelectedMusician == null || PayAmount == 0.0 || PayAmount == null)
                return;


            using (var db = new MusicianDbContext())
            {
                var updateMusician = db.Musicians.FirstOrDefault(m => m.Name == SelectedMusician.Name);
                if (updateMusician == null) return;
                
                if (PayAmount < updateMusician.outBalance)
                {
                    updateMusician.outBalance -= (double)PayAmount;
                    updateMusician.Balance += (double)PayAmount;
                }
                else
                {
                    var saveAmount = updateMusician.outBalance;
                    updateMusician.Balance += updateMusician.outBalance;
                    updateMusician.outBalance -= saveAmount;
                }

                db.SaveChanges();
            }

            using (var db = new KassaDbContext())
            {
                var kassa = db.Kassa.FirstOrDefault();
                if (kassa != null)
                {
                    if(PayAmount>SelectedMusician.outBalance)
                        kassa.ProjectedBalance += (double)PayAmount - SelectedMusician.outBalance;

                    kassa.Balance += (double)PayAmount;
                    kassa.Transactions.Add(new TransactionDB(
                            (double)PayAmount,
                            $"Abrechnung {SelectedMusician.Name}",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                            TransactionType.Einnahme
                        ));
                    db.SaveChanges();
                }
            }
            ShowTopMessage($"{PayAmount} von {SelectedMusician.Name} bezahlt.", MessageIntent.Success);
            Musicians.Remove(SelectedMusician);
            SelectedMusician = null;
        }
    }
}
