using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class CurrencyOperation
    {
        [Key]
        public int CurrencyOperationId { get; set; }

        [ForeignKey("FromAccount")]
        public int FromAccountId { get; set; }

        [ForeignKey("ToAccount")]
        public int ToAccountId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "AmountRequired")]
        public decimal TransferAmount { get; set; }//needs to be transformed into right currency!!!!


        public virtual Account FromAccount { get; set; }
        public virtual Account ToAccount { get; set; }
    }
}