using System.Web.UI.WebControls;
using EasyBank.Filters;
using EasyBank.Models;
using SimpleMembershipTest.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using EasyBank.BAL;


namespace EasyBank.Controllers
{
    [Culture]
    [Authorize(Roles="Administrator, Operator")]
    [InitializeSimpleMembership]
    public class OperationController : Controller
    {
        ConnectionContext db = new ConnectionContext();
        [HttpGet]
        public ActionResult AddMoney(int? id)
        {

            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);

            List<string> currencyNames = (from c in db.Currencies
                                      select c.CurrencyName).ToList();
            List<SelectListItem> itemList = new List<SelectListItem>();
            foreach (string currencyName in currencyNames)
            {
                itemList.Add(new SelectListItem
                {
                    Text = currencyName,
                    Value = currencyName
                });
            }
            ViewBag.itemList = itemList;

            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult AddMoney(int? accountId, decimal? amount, int? clientId, string CurrencyName)
        {
            OperationManager om = new OperationManager();
            CurrencyName = Request["CurrencyName"].ToUpper();
            ErrorCode errorCode = om.DepositMoney(User.Identity.Name, accountId, amount, CurrencyName);
            if (errorCode == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });

            string result = (int)errorCode + " " + ErrorHandler.GetEnumDescription(errorCode);
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId = accountId });
        }

        [HttpGet]
        public ActionResult WidthdrawMoney(int? id)
        {

            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);

            List<string> currencyNames = (from c in db.Currencies
                                          select c.CurrencyName).ToList();
            List<SelectListItem> itemList = new List<SelectListItem>();
            foreach (string currencyName in currencyNames)
            {
                itemList.Add(new SelectListItem
                {
                    Text = currencyName,
                    Value = currencyName
                });
            }
            ViewBag.itemList = itemList;

            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult WidthdrawMoney(int? accountId, decimal? amount, int? clientId, string CurrencyName)
        {
            OperationManager om = new OperationManager();
            ErrorCode errorCode = om.WithdrawMoney(User.Identity.Name, accountId, amount, CurrencyName);
            if (errorCode == ErrorCode.Ok)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });

            string result = (int)errorCode + " " + ErrorHandler.GetEnumDescription(errorCode);
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId = accountId });
        }

        [HttpGet]
        public ActionResult TransferMoney(int? id)
        {

            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);
            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult TransferMoney(int? fromAccountId, string accountNumber, decimal? amount, int? clientId)
        {
            OperationManager om = new OperationManager();
            
            ErrorCode errorCode = om.TransferMoney(User.Identity.Name, fromAccountId, accountNumber, amount);

            if (errorCode == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });

            string result = (int)errorCode + " " + ErrorHandler.GetEnumDescription(errorCode);
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId = fromAccountId });
        }

        public ActionResult TotalDepositedAmount()
        {
            var depositAccounts = (from da in db.Accounts
                                  where da.TypeId != 3
                                  select da).ToList();
            var currencies = (from cur in db.Currencies
                             select cur).ToList();
            List<TotalAmountForCurrency> total = new List<TotalAmountForCurrency>();
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                foreach (var account in depositAccounts)
                {
                    if (account.Currency.CurrencyName == currency.CurrencyName)
                    {
                        totalAmount += account.Amount;
                    }
                }
                total.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName , TotalAmount = totalAmount});
            }
            return View(total);
        }

        public ActionResult TotalDepositInterests()
        {
            var depositInterests = (from di in db.Accounts
                                   where di.TypeId == 2
                                   select di).ToList();
            var currencies = (from cur in db.Currencies
                              select cur).ToList();
            List<TotalAmountForCurrency> total = new List<TotalAmountForCurrency>();
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                foreach (var account in depositInterests)
                {
                    if (account.Currency.CurrencyName == currency.CurrencyName)
                    {
                        totalAmount += account.Interest;
                    }
                }
                total.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount });
            }
            return PartialView(total);
        }

        public ActionResult TotalCreditedAmount()
        {
            var creditAccounts = (from ca in db.Accounts
                                   where ca.TypeId == 3
                                   select ca).ToList();
            var currencies = (from cur in db.Currencies
                              select cur).ToList();
            List<TotalAmountForCurrency> total = new List<TotalAmountForCurrency>();
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                foreach (var account in creditAccounts)
                {
                    if (account.Currency.CurrencyName == currency.CurrencyName)
                    {
                        totalAmount += account.Amount;
                    }
                }
                total.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount });
            }
            return PartialView(total);
        }

        public ActionResult TotalCreditInterests()
        {
            var creditInterests = (from ci in db.Accounts
                                    where ci.TypeId == 3
                                    select ci).ToList();
            var currencies = (from cur in db.Currencies
                              select cur).ToList();
            List<TotalAmountForCurrency> total = new List<TotalAmountForCurrency>();
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                foreach (var account in creditInterests)
                {
                    if (account.Currency.CurrencyName == currency.CurrencyName)
                    {
                        totalAmount += account.Interest;
                    }
                }
                total.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount });
            }
            return PartialView(total);
        }

        [HttpGet]
        public ActionResult ProfitCalc()
        {
            return PartialView();
        }

        // Начисление процентов на депозитные и кредитные счета
        [HttpPost]
        public ActionResult ProfitCalc(ProfitCalc monthsPassed)
        {

            var depositAccountsT = (from depAcc in db.Accounts
                                    where depAcc.TypeId == 2
                                    where depAcc.DepositCreditModel.EarlyTermination == true
                                    select depAcc).ToList();

            DateTime today = DateTime.Now;
            
            foreach (var item in depositAccountsT)
            {
                InterestCalc(item, monthsPassed);
            }

            var depositAccountsF = (from depAcc in db.Accounts
                                    where depAcc.TypeId == 2
                                    where depAcc.DepositCreditModel.EarlyTermination == false
                                    select depAcc).ToList();

            foreach (var item in depositAccountsF)
            {
                InterestCalc(item, monthsPassed);
            }

            var creditAccountsT = (from creds in db.Accounts
                                   where creds.TypeId == 3
                                   where creds.DepositCreditModel.EarlyTermination == true
                                   select creds).ToList();
            
            foreach (var item in creditAccountsT)
            {
                InterestCalc(item, monthsPassed);
            }
            
            return RedirectToAction("TotalDepositedAmount", "Operation");
        }
        // Вывод планированых процентов по депозитам
        public ActionResult TotalPlannedDepositInterests(List<VirtualAccount> total)
        {
            var currencies = (from cur in db.Currencies
                              select cur).ToList();
            List<TotalAmountForCurrency> totalForCurrency = new List<TotalAmountForCurrency>();
            foreach (var currency in currencies)
            {
                decimal totalAmount = 0;
                foreach (var account in total)
                {
                    if (account.CurrencyName == currency.CurrencyName)
                    {
                        totalAmount += account.Interest;
                    }
                }
                totalForCurrency.Add(new TotalAmountForCurrency() { CurrencyName = currency.CurrencyName, TotalAmount = totalAmount });
            }
            return PartialView(totalForCurrency);
        }

        //Поанирование начисления процентов на определенный срок
        [HttpGet]
        public ActionResult ProfitPlanningCalc()
        {
            return PartialView();
        }

        //Планирование начисления процентов на определенный срок
        [HttpPost]
        public ActionResult ProfitPlanningCalc(ProfitCalc monthsPassed)
        {
            List<TotalAmountForCurrency> InterestList = new List<TotalAmountForCurrency>();
            StatisticService SS = new StatisticService();
            InterestList = SS.InterestPlanning(monthsPassed);

            return View("ProfitPlanningCalcView",InterestList);
            
            
         }



        //Метод начисления процентов на депозитные и кредитные счета
        private void InterestCalc (Account item,ProfitCalc monthsPassed )
        {
            DateTime newDate = item.LastInterestAdded.AddMonths(monthsPassed.Months);
            TimeSpan timeSpan = newDate.Subtract(item.LastInterestAdded);
            TimeSpan daysLeft = item.ExpirationDate.Subtract(item.LastInterestAdded);
            TimeSpan zero = daysLeft - daysLeft;
            if (daysLeft > timeSpan)
            {
                decimal timeSpanDec = (decimal)timeSpan.TotalDays;

                decimal interest = item.DepositCreditModel.InterestRate;
                decimal interestForPeriod = interest / 365 * timeSpanDec / 100;

                decimal amountForPeriod = item.Amount * interestForPeriod;

                item.Interest += amountForPeriod;
                item.AvailableAmount = item.Amount + amountForPeriod;

                item.LastInterestAdded = item.LastInterestAdded.AddDays((double)timeSpanDec);


                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else if (daysLeft < timeSpan && daysLeft > zero)
            {
                decimal daysLeftDec = (decimal)daysLeft.TotalDays;

                decimal interest = item.DepositCreditModel.InterestRate;
                decimal interestForPeriod = interest / 365 * daysLeftDec / 100;

                decimal amountForPeriod = item.Amount * interestForPeriod;

                item.Interest += amountForPeriod;
                item.AvailableAmount = item.Amount + amountForPeriod;

                item.LastInterestAdded = item.LastInterestAdded.AddDays((double)daysLeftDec);

                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
