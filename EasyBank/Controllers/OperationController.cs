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
            ConnectionContext db = new ConnectionContext();
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
            int result = om.DepositMoney(User.Identity.Name, accountId, amount, CurrencyName);
            if (result == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId = accountId });
        }

        [HttpGet]
        public ActionResult WidthdrawMoney(int? id)
        {
            ConnectionContext db = new ConnectionContext();
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
            int result = om.WithdrawMoney(User.Identity.Name, accountId, amount, CurrencyName);
            if (result == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId = accountId });
        }

        [HttpGet]
        public ActionResult TransferMoney(int? id)
        {
            ConnectionContext db = new ConnectionContext();
            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);
            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult TransferMoney(int? fromAccountId, string accountNumber, decimal? amount, int? clientId)
        {
            OperationManager om = new OperationManager();
            
            int result = om.TransferMoney(User.Identity.Name, fromAccountId, accountNumber, amount);

            if (result == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });
            return RedirectToAction("OperationError", "Error", new { errorCode = result, AccountId =  fromAccountId });
        }

        public ActionResult TotalDepositedAmount()
        {
            ConnectionContext db = new ConnectionContext();
            decimal[] depositsAmount = (from da in db.Accounts
                                  where da.TypeId != 3
                                  select da.Amount).ToArray();
            decimal totalDeposits = 0;
            foreach (var item in depositsAmount)
            {
               totalDeposits += item;
            }

            return View(totalDeposits);
        }

        public ActionResult TotalDepositInterests()
        {
            ConnectionContext db = new ConnectionContext();
            decimal[] depositInterest = (from di in db.Accounts
                                         where di.TypeId == 2
                                         select di.Interest).ToArray();
            decimal totalInterest = 0;
            foreach (var item in depositInterest)
            {
                totalInterest += item;
            }
            return PartialView(totalInterest);
        }

        public ActionResult TotalCreditedAmount()
        {
            ConnectionContext db = new ConnectionContext();
            decimal[] creditsAmount = (from ca in db.Accounts
                                        where ca.TypeId == 3
                                        select ca.Amount).ToArray();
            decimal totalCredits = 0;
            foreach (var item in creditsAmount)
            {
                totalCredits += item;
            }

            return PartialView(totalCredits);
        }

        public ActionResult TotalCreditInterests()
        {
            ConnectionContext db = new ConnectionContext();
            decimal[] creditInterest = (from ci in db.Accounts
                                        where ci.TypeId == 3
                                        select ci.Interest).ToArray();
            decimal totalInterest = 0;
            foreach (var item in creditInterest)
            {
                totalInterest += item;
            }
            return PartialView(totalInterest);
        }

        [HttpGet]
        public ActionResult ProfitCalc()
        {
            return PartialView();
        }

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
               // InterestCalc(item, monthsPassed);
            }
            
            return RedirectToAction("TotalDepositedAmount", "Operation");
        }

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
