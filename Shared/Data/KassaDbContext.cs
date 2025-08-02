using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Shared.Data
{
    public class KassaDbContext : DbContext
    {
        public DbSet<KassaDB> Kassa { get; set; }
        public DbSet<TransactionDB> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "Kassa.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        //public KassaDbContext(DbContextOptions<KassaDbContext> options)
        //: base(options)
        //{
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Beziehungen, Constraints, Konfiguration
            modelBuilder.Entity<TransactionDB>()
                .HasOne(d => d.Kassa)
                .WithMany(d => d.Transactions)
                .HasForeignKey(d => d.KassaId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TransactionDB>()
                        .Property(e => e.TransactionType)
                        .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
