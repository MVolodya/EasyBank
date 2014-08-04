using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class TotalAmountForCurrency
    {
        public int TotalAmountForCurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}