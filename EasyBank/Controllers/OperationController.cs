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
    [Authorize]
    [InitializeSimpleMembership]
    public class OperationController : Controller
    {
        [HttpGet]
        public ActionResult AddMoney(int? Id)
        {
            ConnectionContext db = new ConnectionContext();
            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == Id);
            if (account != null)
                return View(account);
            else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult AddMoney(int? accountId, int? amount)
        {
            OperationManager om = new OperationManager();
            om.DepositMoney(User.Identity.Name, accountId, amount);
            return RedirectToAction("ClientsProfile", "Protected"/*, new { clientId = client.ClientId }*/);
        }
    }
}
