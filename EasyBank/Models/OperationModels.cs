using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public enum OperationTypes { None = 0, Deposit = 1, Withdraw = 2, Transfer = 3 }

    public class Operation
    {
        [Key]
        public int OperationId { get; set; }
        public DateTime Date { get; set; }
        public OperationTypes Type { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("Operator")]
        public int? OperatorId { get; set; }

        [ForeignKey("FromAccount")]
        public int? FromAccountId { get; set; }
        [ForeignKey("ToAccount")]
        public int? ToAccountId { get; set; }

        public virtual Operator Operator { get; set; }
        public virtual Account FromAccount { get; set; }
        public virtual Account ToAccount { get; set; }

        public Operation() : this(0, OperationTypes.None) { }

        public Operation(int amount, OperationTypes type, int? fromAccountId = null, int? toAccountId = null, int? operatorId = null)
        {
            Date = DateTime.Now;
            Amount = amount;
            Type = type;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            OperatorId = operatorId;
        }
    }
}