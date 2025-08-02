using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Shared.Data
{
    public class KassaDB
    {
        [Key]
        public int Id { get; set; }
        public double Balance { get; set; } = 0.0;
        public double ProjectedBalance { get; set; } = 0.0;
        public IList<TransactionDB> Transactions { get; set; } = new List<TransactionDB>();

        public KassaDB(double balance, double projectedBalance) 
        {
            Balance = balance;
            ProjectedBalance = projectedBalance;
        }
    }
}
