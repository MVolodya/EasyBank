using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class UnaryOperation
    {
        [Key]
        public int OperationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }

    public class DepositOperation : UnaryOperation
    {
    }    

    public class WithdrawOperation : UnaryOperation
    {
    }

    public class BinaryOperation
    {
        [Key]
        public int OperationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Account")]
        public int AccountId { get; set; }
        [ForeignKey("ToAccount")]
        public int ToAccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Account ToAccount { get; set; }
    }

    public class TransferOperation : BinaryOperation
    {        
    }

    public class CurrencOperation : BinaryOperation
    {
    }
}