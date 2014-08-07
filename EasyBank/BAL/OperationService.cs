using System.ComponentModel;
using System.Reflection;
using EasyBank.BAL;
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
        //3 - negative or zero operation amount
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
        private ErrorCode PerformInsideBankMoneyTransfer(ConnectionContext db, Currency sourceCurrency, Currency targetCurrency, decimal amount, ref decimal convertedAmount, decimal? amountThatHasToBeWithdrawn = null)
        {
            if (sourceCurrency == targetCurrency)
            {
                return ErrorCode.Ok;
            }

            BankAccount receivingBankAccount = null;
            BankAccount sendingBankAccount = null;

            receivingBankAccount = db.BankAccounts.FirstOrDefault(a => a.CurrencyName == sourceCurrency.CurrencyName);
            sendingBankAccount = db.BankAccounts.FirstOrDefault(a => a.CurrencyName == targetCurrency.CurrencyName);
            if (receivingBankAccount == null)
            {
                return ErrorCode.NoBankReceivingAccountWithSpecifiedName;
            }
            if (sendingBankAccount == null)
            {
                return ErrorCode.NoBankSendingAccountWithSpecifiedName;
            }

            if (amountThatHasToBeWithdrawn != null)
            {
                amount = (decimal)amountThatHasToBeWithdrawn;
            }

            if (sendingBankAccount.Amount - convertedAmount < 0) return ErrorCode.NotEnoughMoneyOnSendingBankAccount;

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

            return ErrorCode.Ok;
        }

        public ErrorCode DepositMoney(string operatorEmail, int? toAccountId, decimal? amount, string sourceCurrencyName)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return ErrorCode.NullOperatorsEmail;
            /*{
                throw new ArgumentException("Parameter required", "operatorEmail");
            }*/
            if (toAccountId == null)
            {
                return ErrorCode.NullAccountId;
            }
            if (!amount.HasValue)
            {
                return ErrorCode.NullAmount;
            }
            if (sourceCurrencyName == null)
            {
                return ErrorCode.InvalidSourceCurrency;
            }

            if (amount <= 0) 
                return ErrorCode.NegativeOrZeroOperationAmount;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == toAccountId);

            if (oper == null) return ErrorCode.OperatorNotFoundInDb;
            if (acc == null) return ErrorCode.AccountNotFoundInDb;

            if (acc.AccountStatus.StatusName == "Blocked") return ErrorCode.ReceivingAccountIsBlocked;
            if (acc.AccountStatus.StatusName == "Frozen") return ErrorCode.ReceivingAccountIsFrozen;
            if (acc.AccountStatus.StatusName == "Expired") return ErrorCode.ReceivingAccountIsExpired;

            Currency sourceCurrency = db.Currencies.FirstOrDefault(c => c.CurrencyName.ToLower() == sourceCurrencyName.ToLower());
            Currency targetCurrency = acc.Currency;

            if (sourceCurrency == null) return ErrorCode.SourceCurrencyNotFoundInDb;
            if (targetCurrency == null) return ErrorCode.TargetCurrencyNotFoundInDb;

            decimal convertedAmount = GetConvertedAmount(db, amount.Value, sourceCurrency, targetCurrency);

            bool dataChanged = false;

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (amount < 5) return ErrorCode.TooSmallAmountForTA;

                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount);
                        acc.Amount += convertedAmount; //add on main acc
                        acc.AvailableAmount += convertedAmount; // add on available acc

                        dataChanged = true;
                    }
                    break;
                case "Deposit":
                    {
                        if (amount < 100) return ErrorCode.TooSmallAmountForDA;

                        PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount);
                        acc.Amount += (decimal)convertedAmount;
                        acc.AvailableAmount += (decimal)convertedAmount;

                        dataChanged = true;
                    }
                    break;
                case "Credit":
                    {
                        if (acc.Amount - convertedAmount < 0) return ErrorCode.AddingThisAmountWillMakeCredAccNegative;

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
                return ErrorCode.Ok;
            }
            return ErrorCode.UncatchedError;
        }

        public ErrorCode WithdrawMoney(string operatorEmail, int? fromAccountId, decimal? amount, string targetCurrencyName)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return ErrorCode.NullOperatorsEmail;
            if (fromAccountId == null) return ErrorCode.NullAccountId;
            if (amount == null) return ErrorCode.NullAmount;

            if (amount <= 0) return ErrorCode.NegativeOrZeroOperationAmount;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account acc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);

            if (oper == null) return ErrorCode.OperatorNotFoundInDb;
            if (acc == null) return ErrorCode.AccountNotFoundInDb;

            if (acc.AccountStatus.StatusName == "Blocked") return ErrorCode.SendingAccountIsBlocked;
            if (acc.AccountStatus.StatusName == "Frozen") return ErrorCode.SendingAccountIsFrozen;
            if (acc.AccountStatus.StatusName == "Expired") return ErrorCode.SendingAccountIsExpired;

            Currency sourceCurrency = acc.Currency;
            Currency targetCurrency = db.Currencies.FirstOrDefault(c => c.CurrencyName.ToLower() == targetCurrencyName.ToLower());

            if (sourceCurrency == null) return ErrorCode.SourceCurrencyNotFoundInDb;
            if (targetCurrency == null) return ErrorCode.TargetCurrencyNotFoundInDb;

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

            switch (acc.AccountType.TypeName)
            {
                case "Normal"://transfer account
                    {
                        if (acc.AvailableAmount - amountThatHasToBeWithDrawnFromClient < 0) return ErrorCode.NotEnoughMoney;
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
                                if (acc.AvailableAmount - amountThatHasToBeWithDrawnFromClient < 0) return ErrorCode.NotEnoughMoney;
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
                                if (!(acc.AvailableAmount - convertedAmount).Equals(0)) return ErrorCode.IfEarlyTermFalseOnlyWholeAmountCanBeWidthdrawn;
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
                        return ErrorCode.AttemptToWidthdrawFromCreditAcc;
                    }
                    break;
            }
            if (dataChanged)
            {
                db.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                HistoryManager.AddWidthdrawOperation(db, (decimal)amountThatHasToBeWithDrawnFromClient, (int)fromAccountId, (int)oper.OperatorID);
                db.SaveChanges();

                return ErrorCode.Ok;
            }
            return ErrorCode.UncatchedError;
        }
        public ErrorCode TransferMoney(string operatorEmail, int? fromAccountId, string toAccountNumber, decimal? amount)
        {
            ConnectionContext db = new ConnectionContext();

            if (operatorEmail == null) return ErrorCode.NullOperatorsEmail;
            if (fromAccountId == null || toAccountNumber == null) return ErrorCode.NullAccountId;
            if (amount == null) return ErrorCode.NullAmount;

            if (amount <= 0) return ErrorCode.NegativeOrZeroOperationAmount;

            Operator oper = db.Operators.FirstOrDefault(o => o.Email == operatorEmail);
            Account fromAcc = db.Accounts.FirstOrDefault(a => a.AccountId == fromAccountId);
            Account toAcc = db.Accounts.FirstOrDefault(a => a.AccountNumber == toAccountNumber);

            if (oper == null) return ErrorCode.OperatorNotFoundInDb;
            if (fromAcc == null) return ErrorCode.AccountNotFoundInDb;
            if (toAcc == null) return ErrorCode.AccountNotFoundInDb;

            int toAccountId = toAcc.AccountId;

            if (fromAccountId == toAccountId) return ErrorCode.CantTransferMoneyToYourself;

            if (fromAcc.AccountStatus.StatusName == "Blocked") return ErrorCode.SendingAccountIsBlocked;
            if (fromAcc.AccountStatus.StatusName == "Frozen") return ErrorCode.SendingAccountIsFrozen;
            if (fromAcc.AccountStatus.StatusName == "Expired") return ErrorCode.SendingAccountIsExpired;
            if (toAcc.AccountStatus.StatusName == "Blocked") return ErrorCode.ReceivingAccountIsBlocked;
            if (toAcc.AccountStatus.StatusName == "Frozen") return ErrorCode.ReceivingAccountIsFrozen;
            if (toAcc.AccountStatus.StatusName == "Expired") return ErrorCode.ReceivingAccountIsExpired;

            Currency sourceCurrency = fromAcc.Currency;
            Currency targetCurrency = toAcc.Currency;

            if (sourceCurrency == null) return ErrorCode.SourceCurrencyFail;
            if (targetCurrency == null) return ErrorCode.TargetCurrencyFail;

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
                        if (fromAcc.AvailableAmount - amountThatHasToBeWithDrawnFromClient < 0) return ErrorCode.NotEnoughMoney;

                        if (toAcc.AccountType.TypeName == "Normal")
                        {
                            PerformInsideBankMoneyTransfer(db, sourceCurrency, targetCurrency, (decimal)amount, ref convertedAmount, amountThatHasToBeWithDrawnFromClient);

                            fromAcc.AvailableAmount -= (decimal)amountThatHasToBeWithDrawnFromClient;
                            fromAcc.Amount -= (decimal)amountThatHasToBeWithDrawnFromClient;

                            toAcc.AvailableAmount += (decimal)amount;
                            toAcc.Amount += (decimal)amount;

                            dataChanged = true;
                        }
                        else return ErrorCode.AttemptToTransferToNotMoneyTransferAcc;
                    }
                    break;
                default:
                    {
                        return ErrorCode.AttemptToTransferFromNotMoneyTransferAcc;
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
                return ErrorCode.Ok;
            }
            return ErrorCode.UncatchedError;
        }
    }
}