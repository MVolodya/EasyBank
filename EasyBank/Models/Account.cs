using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        [Display(Name = "Account number")]
        public string AccountNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpirationDate { get; set; }

        public decimal Amount { get; set; }


        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required]
        [ForeignKey("AccountType")]
        public int TypeId { get; set; }

        [Required]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        [Required]
        [ForeignKey("AccountStatus")]
        public int StatusId { get; set; }

        public virtual Client Client { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual AccountStatus AccountStatus { get; set; }
        public ICollection<CurrencyOperation> CurrencyOperations { get; set; }
    }
}