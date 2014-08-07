using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.BAL
{
    public class StatisticService
    {
        public List<TotalAmountForCurrency> InterestPlanning(ProfitCalc monthsPassed) 
        {
            ConnectionContext db = new ConnectionContext();
            var depositAccountsT = (from depAcc in db.Accounts
                                    where depAcc.TypeId == 2
                                    where depAcc.DepositCreditModel.EarlyTermination == true
                                    select depAcc).ToList();

            DateTime today = DateTime.Now;
            List<VirtualAccount> total = new List<VirtualAccount>();

            foreach (var item in depositAccountsT)
            {
                InterestPlanningCalc(item, monthsPassed, total);
            }

            var depositAccountsF = (from depAcc in db.Accounts
                                    where depAcc.TypeId == 2
                                    where depAcc.DepositCreditModel.EarlyTermination == false
                                    select depAcc).ToList();

            foreach (var item in depositAccountsF)
            {
                InterestPlanningCalc(item, monthsPassed, total);
            }

            var creditAccountsT = (from creds in db.Accounts
                                   where creds.TypeId == 3
                                   select creds).ToList();
            foreach (var item in creditAccountsT)
            {
                InterestPlanningCalc(item, monthsPassed, total);
            }

           /* var creditAccountsF = (from creds in db.Accounts
                                   where creds.TypeId == 3
                                   where creds.DepositCreditModel.EarlyTermination == false
                                   select creds).ToList();
            foreach (var item in creditAccountsT)
            {
                InterestPlanningCalc(item, monthsPassed, total);
            } */

            var currencies = (from cur in db.Currencies
                              select cur).ToList();
            List<TotalAmountForCurrency> totalForCurrency = new List<TotalAmountForCurrency>();
            
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                int typeId = 0;
                foreach (var account in total)
                {
                    if (account.CurrencyName == currency.CurrencyName & account.TypeId !=3)
                    {
                        totalAmount += account.Interest;
                    }

                }
                totalForCurrency.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount, TypeId = typeId  });
            }
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                int typeId = 3;
                foreach (var account in total)
                {
                    if (account.CurrencyName == currency.CurrencyName & account.TypeId == 3)
                    {
                        totalAmount += account.Interest;
                    }

                }
                totalForCurrency.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount, TypeId = typeId });
            }

            return (totalForCurrency);
        }

        //Метод ПЛАНИРОВАНИЯ начисления процентов на депозитные и кредитные счета
        private static List<VirtualAccount> InterestPlanningCalc(Account item, ProfitCalc monthsPassed, List<VirtualAccount> total)
        {
            DateTime newDate = item.LastInterestAdded.AddMonths(monthsPassed.Months);
            TimeSpan timeSpan = newDate.Subtract(item.LastInterestAdded);
            TimeSpan daysLeft;
            
            if (item.TypeId == 3)
            {
                TimeSpan daysLeftForCredit = item.ExpirationDate.Subtract(DateTime.Now);
                daysLeft = daysLeftForCredit;
            }
            else  
            {
                TimeSpan daysLeftForDeposit = item.ExpirationDate.Subtract(item.LastInterestAdded);
                daysLeft = daysLeftForDeposit;
            }

            TimeSpan zero = daysLeft - daysLeft;

            if (daysLeft > timeSpan)
            {
                decimal timeSpanDec = (decimal)timeSpan.TotalDays;
                decimal interest = item.DepositCreditModel.InterestRate;
                decimal interestForPeriod = Math.Round(interest / 365 * timeSpanDec / 100,2);
                decimal amountForPeriod = Math.Round(item.Amount * interestForPeriod,2);

                total.Add(new VirtualAccount() { CurrencyName = item.Currency.CurrencyName, Interest = amountForPeriod, TypeId = item.TypeId });

                return (total);
            }
            else if (daysLeft < timeSpan && daysLeft > zero)
            {
                decimal daysLeftDec = (decimal)daysLeft.TotalDays;
                decimal interest = item.DepositCreditModel.InterestRate;
                decimal interestForPeriod = Math.Round(interest / 365 * daysLeftDec / 100,2);
                decimal amountForPeriod = Math.Round(item.Amount * interestForPeriod,2);

                total.Add(new VirtualAccount() { CurrencyName = item.Currency.CurrencyName, Interest = amountForPeriod, TypeId = item.TypeId });

                return (total);
            }
            return (total);
        }
    }
}