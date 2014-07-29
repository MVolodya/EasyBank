using EasyBank.DAL;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank
{
    public class OperationManager
    {
        HistoryManager history = new HistoryManager();

        private static bool MoneyCanBeDeposited(Account account)
        {
            if (account != null && account.TypeId != 3 && account.StatusId == 1)
            {
                return true;
            }
            else return false;
        }

        //0 - ok
        //1 - too small amount (min 5)
        //2 - 
        public int DepositMoney(int? operatorId, int? toAccountId, decimal? amount)
        {
            ConnectionContext db = new ConnectionContext();

            

            if (operatorId != null && toAccountId != null && amount != null && amount > 0)
            {
                Operator oper = db.Operators.FirstOrDefault(o => o.OperatorID == operatorId);
                Account account = db.Accounts.FirstOrDefault(a => a.AccountId == toAccountId);

                if (oper == null) return 1;
                else if (account == null) return 2;
                else
                {
                    if (MoneyCanBeDeposited(account))
                    {
                        account.Amount = account.Amount + 10000;
                        db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                        //db.Accounts.First(a => a.AccountId == toAccountId).Amount += account.Amount + (decimal)amount;
                        db.SaveChanges();
                        return 0;
                    }
                    else return 3;
                }
            }
            else return 10;
        }
    }
}