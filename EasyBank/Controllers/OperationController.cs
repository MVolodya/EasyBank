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
            return RedirectToAction("OperationError", "Error", new { errorCode = result });
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
            return RedirectToAction("OperationError", "Error", new { errorCode = result });
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
            return RedirectToAction("OperationError", "Error", new { errorCode = result });
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
    }
}
