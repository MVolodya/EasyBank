using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class VirtualAccount
    {

        public string CurrencyName { get; set; }

        public decimal Interest { get; set; }
        public int TypeId { get; set; }

    }
}