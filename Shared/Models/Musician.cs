using Shared.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Musician
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Satz Instrument { get; set; }
        public double Balance { get; set; } = 0.0;
        public double outBalance { get; set; } = 0.0;
        public IList<DrinkEntryDB> DrinkEntries { get; set; } = new List<DrinkEntryDB>();

        public Musician(string name, Satz instrument)
        {
            Name = name;
            Instrument = instrument;
        }

    }
}
