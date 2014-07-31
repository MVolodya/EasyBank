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
            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult AddMoney(int? accountId, decimal? amount, int? clientId)
        {
            OperationManager om = new OperationManager();
            int result = om.DepositMoney(User.Identity.Name, accountId, amount);
            if (result == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });
            return RedirectToAction("OperationError", "Error", new { errorCode = result });
        }

        [HttpGet]
        public ActionResult WidthdrawMoney(int? id)
        {
            ConnectionContext db = new ConnectionContext();
            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);
            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult WidthdrawMoney(int? accountId, decimal? amount, int? clientId)
        {
            OperationManager om = new OperationManager();
            int result = om.WithdrawMoney(User.Identity.Name, accountId, amount);
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
        public ActionResult TransferMoney(int? fromAccountId, string toAccountNumber, decimal? amount, int? clientId)
        {
            OperationManager om = new OperationManager();
            
            int result = om.TransferMoney(User.Identity.Name, fromAccountId, toAccountNumber, amount);
            if (result == 0)
                return RedirectToAction("ClientsProfile", "Protected", new { clientId = clientId });
            return RedirectToAction("OperationError", "Error", new { errorCode = result });
        }
    }
}
