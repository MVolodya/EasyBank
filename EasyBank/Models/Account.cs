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
        [Key]
        public int AccountId { get; set; }

        [Index("UniqueAccNumb", IsUnique=true)]
        [Display(Name = "AccNumb", ResourceType = typeof(Resources.Resource))]
        [StringLength(10, MinimumLength=10, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "Len8")]
        [RegularExpression(@"^[0-9]+$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        public string AccountNumber { get; set; }

        [Display(ResourceType = typeof(Resources.Resource), Name = "ExpDate")]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "ExpirationDateRequired")]
        [DataType(DataType.Date, ErrorMessageResourceType=typeof(Resources.Resource), ErrorMessageResourceName="DateNotValid")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpirationDate { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Resources.Resource))]
        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "OnlyDigits")]
        public decimal Amount { get; set; }

        [Display(Name = "Client", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "ClientIdRequired")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "TypeIdRequired")]
        [ForeignKey("AccountType")]
        public int TypeId { get; set; }

        [Display(Name = "Currency", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "CurrencyIdRequired")]
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resources.Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "AccountStatusRequired")]
        [ForeignKey("AccountStatus")]
        public int StatusId { get; set; }

        public virtual Client Client { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual AccountStatus AccountStatus { get; set; }
        public virtual ICollection<Operation> OperationsHistory { get; set; }
    }
}