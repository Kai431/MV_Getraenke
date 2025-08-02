using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class DrinkEntryDB
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public double Content { get; set; }
        public string Date { get; set; }

        public int MusicianId { get; set; }
        public Musician Musician { get; set; }

        public DrinkEntryDB() { } // Default constructor for EF Core

        public DrinkEntryDB(DrinkDB drink, string date, Musician musician)
        {
            Name = drink.Name;
            Price = drink.Price;
            Content = drink.Content;
            Date = date;
            Musician = musician;
        }

    }
}
