using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.Icons;

namespace Moolahfy.Models
{
    public class UserData
    {
        /// <summary>
        /// These are the items that the user will check off as they are paid.
        /// </summary>
        public List<Item> WorkingItems { get; set; } = new();

        /// <summary>
        /// These are the items the user adds that the app will automatically add for each month
        /// </summary>
        public List<Item> BaseBudgetItems { get; set; } = new();

        /// <summary>
        /// The last time the user updated anything in the app
        /// </summary>
        public string LastUpdated { get; set; } = DateTime.MinValue.ToShortDateString();

        /// <summary>
        /// Dark mode, obviously...
        /// </summary>
        public bool EnableDarkMode { get; set; } = false;

        public double MonthlyIncome { get; set; }

        public bool HasOnboarded { get; set; }
    }
}
