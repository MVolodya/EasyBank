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

        private void AddMoneyOnDepositAccount(ConnectionContext db, Account account, decimal amount)
        {
            
        }

        //0 - ok

        //1 - too small amount (min 5 for TA)
        //2 - too small amount (min 100 for DA)
        //3 - too small amount (10% of credit for CA)
        //4 - account is Blocked or Frozen or Expired

        //10 - operatorName == null
        //11 - accountId == null
        //12 - amount == null

        //21 - operator not found in db
        //22 - account not found in db


        public int DepositMoney(string operatorEmail, int? toAccountId, decimal? amount)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return 10;
            if (toAccountId == null) return 11;
            if (amount == null) return 12;
                
            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == toAccountId);

            if (oper == null) return 21;
            if (acc == null) return 22;
            
            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (amount < 5) return 1;
                        acc.Amount += (decimal)amount; //add on main acc
                        acc.AvailableAmount += (decimal) amount; // add on available acc
                        db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        HistoryManager.AddDepositOperation(db, (int)amount, (int)toAccountId, (int)oper.OperatorID);
                    }
                    break;
                case "Deposit":
                    {
                        if (amount > 100) return 2;
                        acc.Amount += (decimal)amount;
                        acc.AvailableAmount += (decimal)amount;
                        db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        HistoryManager.AddDepositOperation(db, (int)amount, (int)toAccountId, (int)oper.OperatorID);
                    }
                    break;
                case "Credit":
                    {
                        if (amount > Decimal.Multiply(acc.Amount, (decimal)0.1)) return 3;
                        acc.AvailableAmount += (decimal)amount;
                        db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        HistoryManager.AddDepositOperation(db, (int)amount, (int)toAccountId, (int)oper.OperatorID);
                    }
                    break;
            }
            return 0;
        }
    }
}