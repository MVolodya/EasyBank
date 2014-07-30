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
        //3 - too small amount (10% of credit for CA) //not developed
        //4 - account is Blocked or Frozen or Expired
        //5 - not enough money on account
        //6 - attempt to widthdraw from credit account
        //7 - adding specified amount will make credit acc balance positive(can't be bigger then 0)
        //8 - attempt to transfer from account that is not MT account
        //9 - attempt to transfer to account that is not MT account
        //31 - if early termination is false only whole amount can be widthdrawn

        //10 - operatorName == null
        //11 - accountId == null
        //12 - amount == null

        //21 - operator not found in db
        //22 - account not found in db

        //99 - uncatched error


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

            bool dataChanged = false;

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (amount < 5) return 1;
                        acc.Amount += (decimal)amount; //add on main acc
                        acc.AvailableAmount += (decimal) amount; // add on available acc
                        dataChanged = true;
                    }
                    break;
                case "Deposit":
                    {
                        if (amount > 100) return 2;
                        acc.Amount += (decimal)amount;
                        acc.AvailableAmount += (decimal)amount;
                        dataChanged = true;
                    }
                    break;
                case "Credit":
                    {
                        //if (amount > Decimal.Multiply(acc.Amount, (decimal)0.1)) return 3;
                        if (acc.Amount + amount > 0) return 7;
                        acc.Amount += (decimal)amount;
                        acc.AvailableAmount += (decimal)amount;
                        dataChanged = true;
                    }
                    break;
            }
            if(dataChanged)
            {
                db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                HistoryManager.AddDepositOperation(db, (int)amount, (int)toAccountId, (int)oper.OperatorID);
                db.SaveChanges();
                return 0;
            }
            return 99;
        }

        public int WithdrawMoney(string operatorEmail, int? fromAccountId, decimal? amount)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return 10;
            if (fromAccountId == null) return 11;
            if (amount == null) return 12;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);

            if (oper == null) return 21;
            if (acc == null) return 22;

            bool dataChanged = false;

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (acc.AvailableAmount - amount < 0) return 5;

                        acc.AvailableAmount -= (decimal)amount;
                        acc.Amount -= (decimal)amount;
                        dataChanged = true;
                    }
                    break;
                case "Deposit":
                    {
                        if (DateTime.Now < acc.ExpirationDate)
                        {
                            if (acc.DepositCreditModel.EarlyTermination == true)
                            {
                                if (acc.AvailableAmount - amount < 0) return 5;
                                else
                                {
                                    acc.Amount -= (decimal)amount;
                                    acc.AvailableAmount -= (decimal)amount;
                                    dataChanged = true;
                                }
                            }
                            else
                            {
                                if(!(acc.AvailableAmount - amount).Equals(0))return 31;
                                else
                                {
                                    acc.Amount -=(decimal)amount;
                                    acc.AvailableAmount-=(decimal)amount;
                                    dataChanged = true;
                                }
                            }
                        }
                        else
                        {
                            acc.Amount -= (decimal)amount;
                            acc.AvailableAmount -= (decimal)amount;
                            dataChanged = true;
                        }
                    }
                    break;
                case "Credit":
                    {
                        return 6;
                    }
                    break;
            }
            if (dataChanged)
            {
                db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                HistoryManager.AddDepositOperation(db, (int)amount, (int)fromAccountId, (int)oper.OperatorID);
                db.SaveChanges();
                
                return 0;
            }
            return 99;
        }
        public int TransferMoney(string operatorEmail, int? fromAccountId, string toAccountNumber, decimal? amount)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return 10;
            if (fromAccountId == null || toAccountNumber == null) return 11;
            if (amount == null) return 12;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account fromAcc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);
            Account toAcc = db.Accounts.FirstOrDefault(a => a.AccountNumber == toAccountNumber);
            int toAccountId = toAcc.AccountId;

            if (oper == null) return 21;
            if (fromAcc == null) return 22;
            if (toAcc == null) return 22;

            bool dataChanged = false;

            switch (fromAcc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (fromAcc.AvailableAmount - amount < 0) return 5;

                        if (toAcc.AccountType.TypeName == "Normal")
                        {
                            fromAcc.AvailableAmount -= (decimal)amount;
                            fromAcc.Amount -= (decimal)amount;

                            toAcc.AvailableAmount += (decimal)amount;
                            toAcc.Amount += (decimal)amount;

                            dataChanged = true;
                        }
                        else return 9;
                    }
                    break;
                default:
                    {
                        return 8;
                    }
                    break;
            }
            if (dataChanged)
            {
                db.Entry(fromAcc).State = System.Data.Entity.EntityState.Modified;
                db.Entry(toAcc).State = System.Data.Entity.EntityState.Modified;
                HistoryManager.AddTransferOperation(db, (int)amount, (int)fromAccountId, (int)toAccountId, (int)oper.OperatorID);
                db.SaveChanges();
                return 0;
            }
            return 99;
        }
    }
}