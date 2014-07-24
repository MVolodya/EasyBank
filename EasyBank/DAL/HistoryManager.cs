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
        ConnectionContext db = new ConnectionContext();
        public bool AddDepositOperation(Account account, int amount)
        {
            try
            {
                db.DepositHistory.Add((DepositOperation)new UnaryOperation(account, amount));
                db.SaveChanges();
                return true;
            }
            catch
            {
                Debug.WriteLine("Problem with adding to history");
                return false;
            }
        }
    }
}