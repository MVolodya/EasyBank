﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class BankAccount
    {
        [Key]
        public int BankAccountId { get; set; }

        public int CurrencyName { get; set; }
        public int Amount { get; set; }
    }
}