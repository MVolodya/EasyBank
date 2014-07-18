﻿using System;
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

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "AccountNumberRequired")]
        [StringLength(8, MinimumLength=8, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "Len8")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        [Display(Name = "Account number")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "ExpirationDateRequired")]
        [DataType(DataType.Date, ErrorMessageResourceType=typeof(Resources.Resource), ErrorMessageResourceName="DateNotValid")]
        [Display(Name = "Expiration Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpirationDate { get; set; }

        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        public decimal Amount { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "ClientIdRequired")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "TypeIdRequired")]
        [ForeignKey("AccountType")]
        public int TypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "CurrencyIdRequired")]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "AccountStatusRequired")]
        [ForeignKey("AccountStatus")]
        public int StatusId { get; set; }

        public virtual Client Client { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual AccountStatus AccountStatus { get; set; }
        public ICollection<CurrencyOperation> CurrencyOperations { get; set; }
    }
}