using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public interface Operation
    {
        int OperationId { get; set; }
        decimal Amount { get; set; }
        DateTime Date { get; set; }
        int AccountId { get; set; }
    }

    public class DepositOperation : Operation
    {
        [Key]
        public int OperationId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }

    public class TransferOperation : Operation
    {
        [Key]
        public int OperationId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        [ForeignKey("ToAccount")]
        public int ToAccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Account ToAccount { get; set; }
    }

    public class WithdrawOperation : Operation
    {
        [Key]
        public int OperationId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}