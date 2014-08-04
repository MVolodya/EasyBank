using System;
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

        public string CurrencyName { get; set; }
        public decimal Amount { get; set; }
    }
}