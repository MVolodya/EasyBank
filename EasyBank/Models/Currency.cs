using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [StringLength(10, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "Len10", MinimumLength = 2)]
        [Index("UniqueCurrencyName", IsUnique = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "CurrencyNameRequired")]
        public string CurrencyName { get; set; }

        [Display(Name="Purchase rate")]
        [Required(ErrorMessage = "Purchase rate field is required")]
        [RegularExpression(@"^\d+(.\d{0,2})?$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "DecimalFormat")]
        public decimal PurchaseRate { get; set; }

        [Display(Name = "Sale rate")]
        [Required(ErrorMessage = "Purchase rate field is required")]
        [RegularExpression(@"^\d+(.\d{0,2})?$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "DecimalFormat")]
        public decimal SaleRate { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}