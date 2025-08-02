using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class TransactionDB
    {
        [Key]
        public int Id { get; set; }
        public double Amount { get; set; }           // Positiv = Einnahme, Negativ = Ausgabe
        public string Description { get; set; }
        public string Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public int KassaId { get; set; }            // Foreign Key zu KassaDB
        public KassaDB Kassa { get; set; }          // Navigation Property

        public TransactionDB(double amount, string description, string date, TransactionType transactionType)
        {
            Amount = amount;
            Description = description;
            Date = date;
            TransactionType = transactionType;
        }
    }
    public enum TransactionType
    {
        Einnahme,
        Ausgabe
    }
}
