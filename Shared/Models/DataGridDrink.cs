using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class DataGridDrink
    {
        public string Name { get; }
        public int Count { get; }
        public double ContentSum { get; }
        public double CostSum { get; }
        public DataGridDrink(DrinkDB drink, int count, double contentSum, double costSum) 
        {
            Count = count;
            Name = drink.Name;
            ContentSum = contentSum;
            CostSum = costSum;
        }
    }
}
