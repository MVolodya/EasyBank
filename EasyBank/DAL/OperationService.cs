using EasyBank.DAL;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank
{
    public class OperationService
    {
        ConnectionContext db = new ConnectionContext();
        HistoryManager historyManager = new HistoryManager();
        public static bool FundsCanBeAdded(Account account)
        {
            if (account != null)
            {
                if ((account.AccountType.TypeName == "Normal" || account.AccountType.TypeName == "Credit") && account.AccountStatus.StatusName == "Normal")
                    return true;
            }
            return false;
        }

        public static bool FundsCanBeTransfered(Account account)
        {
            if (account != null)
            {
                if ((account.AccountType.TypeName == "Normal") && account.AccountStatus.StatusName == "Normal")
                    return true;
            }
            return false;
        }

        public static bool FundsCanBeWithdrawn(Account account)
        {
            if (account != null)
            {
                if ((account.AccountType.TypeName == "Normal") && account.AccountStatus.StatusName == "Normal")
                    return true;
            }
            return false;
        }

        public bool AddFunds(int id, int amount)
        {
            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);
            if (account != null)
            {
                if (FundsCanBeAdded(account))
                {
                    historyManager.AddDepositOperation(account, amount);
                    db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}