using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class DrinksDbContext : DbContext
    {
        public DbSet<DrinkDB> Drink { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "Drinks.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        //public KassaDbContext(DbContextOptions<KassaDbContext> options)
        //: base(options)
        //{
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Beziehungen, Constraints, Konfiguration
            base.OnModelCreating(modelBuilder);
        }
    }
}
