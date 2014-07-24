using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class Operation
    {
        [Key]
        public int OperationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("FromAccount")]
        public int FromAccountId { get; set; }
        [ForeignKey("ToAccount")]
        public int ToAccountId { get; set; }

        public virtual Account FromAccount { get; set; }
        public virtual Account ToAccount { get; set; }
    }

    public class DepositOperation : Operation
    {
    }    

    public class WithdrawOperation : Operation
    {
    }

    public class TransferOperation : Operation
    {        
    }

    public class CurrencOperation : Operation
    {
    }
}