using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EasyBank.DAL
{
    public class HistoryManager
    {
        public static void AddDepositOperation(ConnectionContext db, int amount, int toId, int operatorId)
        {
            db.OperationHistory.Add(new Operation(amount, OperationTypes.Deposit, toAccountId:toId, operatorId:operatorId));
            db.SaveChanges();
        }
    }
}