using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class DepositCreditModel
    {
        [Key]
        public int DepositCreditModelID { get; set; }

        [Display(Name="Product Name")]
        public string Name { get; set; }

        [Display(Name="Product Duration")]
        public int Duration { get; set; }

        [Display(Name="Interest")]
        public decimal InterestRate { get; set; }

        [Display(Name="Early Termination")]
        public bool EarlyTermination { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resources.Resource))]
        [ForeignKey("AccountType")]
        public int AccountTypeId { get; set; }

        public virtual ICollection<Account> Account { get; set; }
        public virtual AccountType AccountType { get; set; }
    }
}