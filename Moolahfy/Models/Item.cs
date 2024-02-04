using Moolahfy.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moolahfy.Models
{
    [Serializable]
    public class Item
    {
        public int id { get; set; }
        public int BaseID { get; set; }

        /// <summary>
        /// The date the item is due for payment.
        /// </summary>
        public DateTime Date { get; set; }

        public int DayOfMonth { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Periods Period { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateCompleted { get; set; }
        public bool Deleted { get; set; }
    }
}
