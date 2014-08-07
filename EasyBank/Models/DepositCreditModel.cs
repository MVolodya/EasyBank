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

        [Required(ErrorMessage = "The {0} is required")]
        [Display(Name="Product Name")]
        public string Name { get; set; }

        [Range(1,24)]
        [Display(Name="Product Duration")]
        [Required(ErrorMessage = "The {0} is required")]
        public int Duration { get; set; }

        [Range(1,100)]
        [Display(Name="Interest")]
        [Required(ErrorMessage = "The {0} is required")]
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