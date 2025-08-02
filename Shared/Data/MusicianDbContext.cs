using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class MusicianDbContext : DbContext
    {
        public DbSet<Musician> Musicians { get; set; }
        public DbSet<DrinkEntryDB> DrinkEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "Kühlschranknutzung2025.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            //optionsBuilder.UseSqlite("Data Source=data/Kühlschranknutzung2025.db");
        }
        //public MusicianDbContext(DbContextOptions<MusicianDbContext> options)
        //: base(options)
        //{
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Beziehungen, Constraints, Konfiguration
            modelBuilder.Entity<DrinkEntryDB>()
                .HasOne(d => d.Musician)
                .WithMany(d => d.DrinkEntries)
                .HasForeignKey(d => d.MusicianId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Musician>()
                        .Property(e => e.Instrument)
                        .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
