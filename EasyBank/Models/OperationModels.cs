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
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("Operator")]
        public int? OperatorId { get; set; }
        [ForeignKey("Account")]
        public int? AccountId { get; set; }

        public virtual Operator Operator { get; set; }
        public virtual Account Account { get; set; }

        public Operation()
        {
            Date = DateTime.Now;
        }
        public Operation(int accountId, int amount, int? operatorId = null)
        {
            Date = DateTime.Now;
            AccountId = accountId;
            Amount = amount;
            OperatorId = operatorId;
        }
    }

    public class DepositOperation : Operation
    {
        public DepositOperation():base() { }
        public DepositOperation(int accountId, int amount, int? operatorId = null) : base(accountId, amount, operatorId) { }
    }

    public class WithdrawOperation : Operation
    {
        public WithdrawOperation():base() { }
        public WithdrawOperation(int accountId, int amount, int? operatorId = null) : base(accountId, amount, operatorId) { }
    }

    public class TransferOperation : Operation
    {
        [ForeignKey("ToAccount")]
        public int ToAccountId { get; set; }

        public virtual Account ToAccount { get; set; }

        public TransferOperation():base() { }
        public TransferOperation(int fromAccountId, int toAccountId, int amount, int? operatorId = null):base(fromAccountId, amount, operatorId)
        {
            ToAccountId = toAccountId;
        }
    }
}