using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Shared.Data;

namespace ManagementWeb.Components.ViewModels
{
    public class DrinksViewModel
    {
        private List<DrinkDB> _drinks;

        public string txtName { get; set; }
        public double txtContent { get; set; }
        public double txtPrice { get; set; }
        public DrinkDB? SelectedDrink { get; set; }
        public int txtStockCount { get; set; } = 0;
        public bool SaveButtonDisabled { get; set; } = true;

        public List<DrinkDB> Drinks 
        { 
            get
            {
                return _drinks;
            }
            set
            {
                _drinks = value;
            }
        }

        private readonly IMessageService _messageService;
        public DrinksViewModel(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void Initialize()
        {
            using (var db = new DrinksDbContext())
            {
                var drinks = db.Drink.AsNoTracking().ToList();
                foreach (var drink in drinks)
                    drink.SetImagePath();

                Drinks = drinks;
            }

            SetSelectedDrink(Drinks.FirstOrDefault());
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

        public void SetSelectedDrink(DrinkDB? newDrink)
        {
            if (newDrink == null)
                newDrink = new DrinkDB("", 0.0, 0.0, false);
            else
                SaveButtonDisabled = false;

            SelectedDrink = newDrink;
            SelectedDrink.SetImagePath();
        }

        public void SaveDrink()
        {
            if (SelectedDrink == null)
                return;

            using (var db = new DrinksDbContext())
            {
                var drinkToChange = db.Drink.FirstOrDefault(d => d.Name == SelectedDrink.Name);
                if (drinkToChange == null)
                    return;

                drinkToChange.Content = SelectedDrink.Content;
                drinkToChange.Price = SelectedDrink.Price;
                db.SaveChanges();
            }

            ShowTopMessage("Drink changed and saved!", MessageIntent.Success);
        }

        public void StateChanged(DrinkDB drinkData)
        {
            using (var db = new DrinksDbContext())
            {
                var dbDrink = db.Drink.FirstOrDefault(m => m.Name == drinkData.Name);
                dbDrink.IsListed = !drinkData.IsListed;
                db.SaveChanges();
            }
        }

        public void AddStock()
        {
            if (SelectedDrink == null)
                return;
            using (var db = new DrinksDbContext())
            {
                var dbDrink = db.Drink.FirstOrDefault(m => m.Name == SelectedDrink.Name);
                if (dbDrink != null)
                {
                    dbDrink.StockCount += txtStockCount;
                    SelectedDrink.StockCount += txtStockCount;
                    db.SaveChanges();
                }
            }
        }

        public void SubStock()
        {
            if (SelectedDrink == null)
                return;
            using (var db = new DrinksDbContext())
            {
                var dbDrink = db.Drink.FirstOrDefault(m => m.Name == SelectedDrink.Name);
                if (dbDrink != null)
                {
                    dbDrink.StockCount -= txtStockCount;
                    SelectedDrink.StockCount -= txtStockCount;
                    db.SaveChanges();
                }
            }
        }

        public void SetStock()
        {
            if (SelectedDrink == null)
                return;
            using (var db = new DrinksDbContext())
            {
                var dbDrink = db.Drink.FirstOrDefault(m => m.Name == SelectedDrink.Name);
                if (dbDrink != null)
                {
                    dbDrink.StockCount = txtStockCount;
                    SelectedDrink.StockCount = txtStockCount;
                    db.SaveChanges();
                }
            }
        }
    }
}
