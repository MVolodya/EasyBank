﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class TotalAmountForCurrency
    {
        public string CurrencyName { get; set; }
        public decimal TotalAmount { get; set; }
        public int TypeId { get; set; }
    }
}