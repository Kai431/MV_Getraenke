using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Kassa
    {
        public double Balance { get; private set; }
        public double ProjectedBalance { get; private set; }

        public Kassa(double balance, double projectedBalance)
        {
            Balance = balance;
            ProjectedBalance = projectedBalance;
        }

        public void ChangeBalance(double amount)
        {
            Balance += amount;
            if(amount < 0)
            {
                ProjectedBalance += amount;
            }
            else
                ProjectedBalance -= amount;
        }

        public void AddToProjected(double amount)
        {
            ProjectedBalance += amount;
        }
    }
}
