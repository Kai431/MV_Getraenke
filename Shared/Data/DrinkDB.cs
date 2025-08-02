using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class DrinkDB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Content { get; set; }
        public bool IsListed { get; set; }
        public int StockCount { get; set; }

        private Bitmap? _imagePath;
        public Bitmap? ImagePath { get { return _imagePath; } } 

        private string? _imagePathForHTML;
        public string? ImagePathForHTML
        {
            get { return _imagePathForHTML; }
        }

        public DrinkDB() { } // Default constructor for EF Core
        public DrinkDB(string name, double price, double content, bool isListed)
        {
            Name = name;
            Price = price;
            Content = content;
            IsListed = isListed;
            SetImagePath();
        }

        public void SetImagePath()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, $"Assets/Icons/Drinks/{Name}.png");
                _imagePathForHTML = $"Assets\\Icons\\Drinks\\{Name}.png";
                var imageBytes = File.ReadAllBytes(path);
                using var stream = new MemoryStream(imageBytes);
                _imagePath = new Bitmap(stream);
            }
            catch
            {
                //Do Nothing
            }
        }
    }
}
