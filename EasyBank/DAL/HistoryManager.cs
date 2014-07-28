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
        public static void AddDepositOperation(ConnectionContext db, int fromId, int toId, int amount)
        {
            db.DepositHistory.Add(new DepositOperation(fromId, toId, amount));
            db.SaveChanges();
        }
    }
}