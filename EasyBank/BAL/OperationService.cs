using EasyBank.DAL;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace EasyBank
{
    public class OperationManager
    {
        private HistoryManager history = new HistoryManager();



        private const int Ok = 0;

        //1 - too small amount (min 5 for TA)
        //2 - too small amount (min 100 for DA)
        //3 - negative operation amount
        //--------3 - too small amount (10% of credit for CA) //not developed
        //4 - account is Blocked or Frozen or Expired
        //5 - not enough money on account
        //6 - attempt to widthdraw from credit account
        //7 - adding specified amount will make credit acc balance negative(can't be smaller then 0)
        //8 - attempt to transfer from account that is not MT account
        //9 - attempt to transfer to account that is not MT account
        //31 - if early termination is false only whole amount can be widthdrawn
        //32 - can't transfer money to yourself

        //10 - operatorName == null
        //11 - accountId == null
        //12 - amount == null
        //13 - invalid sourceCurrency
        //14 - invalid targetCurrency

        //21 - operator not found in db
        //22 - account not found in db
        private const int SourceCurrencyNotFound_ERROR_CODE = 23; //sourceCurrency not found in db
        //24 - targetCurrency not found in db
        //25 - sourceCurrency fail
        //26 - targetCurrency fail

        //41 - Sending account is blocked
        //42 - Sending account is frozen
        //43 - Sending account is expired
        //44 - Receiving account is blocked
        //45 - Receiving account is frozen
        //46 - Receiving account is expired

        //50 - No bank receiving account with specified name
        //51 - No bank sending account with specified name
        //55 - Not enough money on sending bank account


        //99 - uncatched error

        private decimal GetConvertedAmount(ConnectionContext db, decimal amount, Currency sourceCurrency, Currency targetCurrency)
        {
            if (sourceCurrency.CurrencyName == targetCurrency.CurrencyName)
                return amount;
            else
            {
                if (sourceCurrency.CurrencyName != "UAH")
                    amount = Math.Round(amount * sourceCurrency.PurchaseRate, 2);

                if (targetCurrency.CurrencyName != "UAH")
                {
                    amount = Math.Round(amount / targetCurrency.SaleRate, 2);
                }
            }
            return amount;
        }

        /// <summary>
        /// Transfers money between internal bank accounts
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sourceCurrency"></param>
        /// <param name="targetCurrency"></param>
        /// <param name="amount"></param>
        /// <param name="convertedAmount"></param>
        /// <returns></returns>
        private int PerformInsideBankMoneyTransfer(ConnectionContext db, Currency sourceCurrency, Currency targetCurrency, decimal amount, ref decimal convertedAmount, decimal? amountThatHasToBeWithdrawn = null)
        {
            if (sourceCurrency == targetCurrency)
            {
                return 0;
            }

            BankAccount receivingBankAccount = null;
            BankAccount sendingBankAccount = null;

            receivingBankAccount = db.BankAccounts.FirstOrDefault(a => a.CurrencyName == sourceCurrency.CurrencyName);
            sendingBankAccount = db.BankAccounts.FirstOrDefault(a => a.CurrencyName == targetCurrency.CurrencyName);
            if (receivingBankAccount == null)
            {
                return 50;
            }
            if (sendingBankAccount == null)
            {
                return 51;
            }

            if (amountThatHasToBeWithdrawn != null)
            {
                amount = (decimal)amountThatHasToBeWithdrawn;
            }

            if (sendingBankAccount.Amount - convertedAmount < 0) return 55;

            if (sourceCurrency.CurrencyName != "UAH" && targetCurrency.CurrencyName != "UAH")
            {
                Currency uahCurrency = db.Currencies.FirstOrDefault(c => c.CurrencyName == "UAH");//test for null
                BankAccount uahBankAccount = db.BankAccounts.FirstOrDefault((a => a.CurrencyName == "UAH"));
                receivingBankAccount.Amount += amount;

                decimal receivedAmountInUah = GetConvertedAmount(db, (decimal)amount, sourceCurrency, uahCurrency);
                //uahBankAccount.Amount += receivedAmountInUah;

                decimal sendingAmount = GetConvertedAmount(db, receivedAmountInUah, uahCurrency, targetCurrency);
                sendingBankAccount.Amount -= sendingAmount;
                convertedAmount = sendingAmount;

                db.Entry(uahBankAccount).State = System.Data.Entity.EntityState.Modified;
                db.Entry(receivingBankAccount).State = System.Data.Entity.EntityState.Modified;
                db.Entry(sendingBankAccount).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                receivingBankAccount.Amount += amount;
                sendingBankAccount.Amount -= convertedAmount;
                db.Entry(receivingBankAccount).State = System.Data.Entity.EntityState.Modified;
                db.Entry(sendingBankAccount).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return 0;
        }

        public int DepositMoney(string operatorEmail, int? toAccountId, decimal? amount, string sourceCurrencyName)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return 10;
            /*{
                throw new ArgumentException("Parameter required", "operatorEmail");
            }*/
            if (toAccountId == null) return 11;
            if (!amount.HasValue) return 12;
            if (sourceCurrencyName == null) return 13;

            if (amount <= 0) return 3;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == toAccountId);

            if (oper == null) return 21;
            if (acc == null) return 22;

            if (acc.AccountStatus.StatusName == "Blocked") return 44;
            if (acc.AccountStatus.StatusName == "Frozen") return 45;
            if (acc.AccountStatus.StatusName == "Expired") return 46;

            Currency sourceCurrency = db.Currencies.FirstOrDefault(c => c.CurrencyName.ToLower() == sourceCurrencyName.ToLower());
            Currency targetCurrency = acc.Currency;

            if (sourceCurrency == null) return 23;
            if (targetCurrency == null) return 25;

            decimal convertedAmount = GetConvertedAmount(db, amount.Value, sourceCurrency, targetCurrency);

            bool dataChanged = false;

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (amount < 5) return 1;

                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount);
                        acc.Amount += convertedAmount; //add on main acc
                        acc.AvailableAmount += convertedAmount; // add on available acc

                        dataChanged = true;
                    }
                    break;
                case "Deposit":
                    {
                        if (amount < 100) return 2;

                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount);
                        acc.Amount += (decimal)convertedAmount;
                        acc.AvailableAmount += (decimal)convertedAmount;

                        dataChanged = true;
                    }
                    break;
                case "Credit":
                    {
                        if (acc.Amount - convertedAmount < 0) return 7;

                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount);
                        acc.Amount -= (decimal)convertedAmount;
                        dataChanged = true;
                    }
                    break;
            }
            if (dataChanged)
            {
                db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                HistoryManager.AddDepositOperation(db, (decimal)amount, (int)toAccountId, (int)oper.OperatorID);
                db.SaveChanges();
                return 0;
            }
            return 99;
        }

        public int WithdrawMoney(string operatorEmail, int? fromAccountId, decimal? amount, string targetCurrencyName)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return 10;
            if (fromAccountId == null) return 11;
            if (amount == null) return 12;

            if (amount <= 0) return 3;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);

            if (oper == null) return 21;
            if (acc == null) return 22;

            if (acc.AccountStatus.StatusName == "Blocked") return 41;
            if (acc.AccountStatus.StatusName == "Frozen") return 42;
            if (acc.AccountStatus.StatusName == "Expired") return 43;

            Currency sourceCurrency = acc.Currency;
            Currency targetCurrency = db.Currencies.FirstOrDefault(c => c.CurrencyName.ToLower() == targetCurrencyName.ToLower());

            if (sourceCurrency == null) return 26;
            if (targetCurrency == null) return 24;

            decimal convertedAmount = (decimal)amount;
            decimal? amountThatHasToBeWithDrawnFromClient = null;
            if (targetCurrency.CurrencyName == sourceCurrency.CurrencyName)
                amountThatHasToBeWithDrawnFromClient = amount;
            else if (sourceCurrency.CurrencyName == "UAH" || targetCurrency.CurrencyName == "UAH")
            {
                if (sourceCurrency.CurrencyName == "UAH")
                    amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount * targetCurrency.SaleRate, 2);
                if (targetCurrency.CurrencyName == "UAH")
                    amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount/sourceCurrency.PurchaseRate, 2);
            }
            else
            {
                amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount*targetCurrency.SaleRate, 2);
                amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amountThatHasToBeWithDrawnFromClient/sourceCurrency.PurchaseRate, 2);

                convertedAmount = GetConvertedAmount(db, (decimal) amount, targetCurrency, sourceCurrency);
            }

            bool dataChanged = false;

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (acc.AvailableAmount - convertedAmount < 0) return 5;
                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);
                        acc.AvailableAmount -= (decimal)amountThatHasToBeWithDrawnFromClient;
                        acc.Amount -= (decimal)amountThatHasToBeWithDrawnFromClient;
                        dataChanged = true;
                    }
                    break;
                case "Deposit":
                    {
                        if (acc.LastInterestAdded < acc.ExpirationDate)
                        {
                            if (acc.DepositCreditModel.EarlyTermination == true)
                            {
                                if (acc.AvailableAmount - convertedAmount < 0) return 5;
                                else
                                {
                                    PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);
                                    acc.Amount -= (decimal)amount;
                                    acc.AvailableAmount -= (decimal)amount;
                                    dataChanged = true;
                                }
                            }
                            else
                            {
                                if (!(acc.AvailableAmount - convertedAmount).Equals(0)) return 31;
                                else
                                {
                                    PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);
                                    //acc.Amount -= (decimal)amount;
                                    acc.Amount = 0;
                                    acc.AvailableAmount -= (decimal)amount;
                                    dataChanged = true;
                                }
                            }
                        }
                        else
                        {
                            PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);
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
                db.SaveChanges();
                HistoryManager.AddWidthdrawOperation(db, (decimal)amountThatHasToBeWithDrawnFromClient, (int)fromAccountId, (int)oper.OperatorID);
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

            if (amount <= 0) return 3;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account fromAcc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);
            Account toAcc = db.Accounts.FirstOrDefault(a => a.AccountNumber == toAccountNumber);

            if (oper == null) return 21;
            if (fromAcc == null) return 22;
            if (toAcc == null) return 22;

            int toAccountId = toAcc.AccountId;

            if (fromAccountId == toAccountId) return 32;

            if (fromAcc.AccountStatus.StatusName == "Blocked") return 41;
            if (fromAcc.AccountStatus.StatusName == "Frozen") return 42;
            if (fromAcc.AccountStatus.StatusName == "Expired") return 43;
            if (toAcc.AccountStatus.StatusName == "Blocked") return 44;
            if (toAcc.AccountStatus.StatusName == "Frozen") return 45;
            if (toAcc.AccountStatus.StatusName == "Expired") return 46;

            Currency sourceCurrency = fromAcc.Currency;
            Currency targetCurrency = toAcc.Currency;

            if (sourceCurrency == null) return 25;
            if (targetCurrency == null) return 26;

            decimal convertedAmount = (decimal)amount;
            decimal? amountThatHasToBeWithDrawnFromClient = null;
            if (targetCurrency.CurrencyName == sourceCurrency.CurrencyName)
                amountThatHasToBeWithDrawnFromClient = amount;
            else if (sourceCurrency.CurrencyName == "UAH" || targetCurrency.CurrencyName == "UAH")
            {
                if (sourceCurrency.CurrencyName == "UAH")
                    amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount * targetCurrency.SaleRate, 2);
                if (targetCurrency.CurrencyName == "UAH")
                    amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount / sourceCurrency.PurchaseRate, 2);
            }
            else
            {
                amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amount * targetCurrency.SaleRate, 2);
                amountThatHasToBeWithDrawnFromClient = Math.Round((decimal)amountThatHasToBeWithDrawnFromClient / sourceCurrency.PurchaseRate, 2);

                convertedAmount = GetConvertedAmount(db, (decimal)amount, targetCurrency, sourceCurrency);
            }

            bool dataChanged = false;

            switch (fromAcc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (fromAcc.AvailableAmount - convertedAmount < 0) return 5;

                        if (toAcc.AccountType.TypeName == "Normal")
                        {
                            PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);

                            fromAcc.AvailableAmount -= (decimal)amountThatHasToBeWithDrawnFromClient;
                            fromAcc.Amount -= (decimal)amountThatHasToBeWithDrawnFromClient;

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
                db.SaveChanges();
                db.Entry(toAcc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HistoryManager.AddTransferOperation(db, (decimal)amountThatHasToBeWithDrawnFromClient, (int)fromAccountId, (int)toAccountId, (int)oper.OperatorID);
                db.SaveChanges();
                return 0;
            }
            return 99;
        }
    }
}