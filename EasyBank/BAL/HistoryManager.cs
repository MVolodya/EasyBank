using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EasyBank.DAL
{
    public class HistoryManager //Has no db.SaveChanges()!!
    {
        public static void AddDepositOperation(ConnectionContext db, decimal amount, int toId, int operatorId)
        {
            db.OperationHistory.Add(new Operation(amount, OperationTypes.Deposit, toAccountId:toId, operatorId:operatorId));
        }

        public static void AddWidthdrawOperation(ConnectionContext db, decimal amount, int fromId, int operatorId)
        {
            db.OperationHistory.Add(new Operation(-amount, OperationTypes.Withdraw, fromAccountId: fromId, operatorId: operatorId));
        }

        public static void AddTransferOperation(ConnectionContext db, decimal amount, int fromId, int toId, int operatorId)
        {
            if (operatorId == 0)
            {
                db.OperationHistory.Add(new Operation(amount, OperationTypes.Transfer, fromId, toId, null));
            }else
            db.OperationHistory.Add(new Operation(amount, OperationTypes.Transfer, fromId, toId, operatorId));
        }
    }
}