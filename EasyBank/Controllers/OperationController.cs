using EasyBank.Filters;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBank.Controllers
{
    [Culture]
    public class OperationController : Controller
    {
        //
        // GET: /Operation/
        ConnectionContext db = new ConnectionContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddMoney(int? accountId)
        {
            if (accountId != null)
            {
                db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                return View(db.Accounts.FirstOrDefault(a => a.AccountId == accountId));
            }
            else return HttpNotFound();
        }

        [HttpPost]
        public ActionResult AddMoney(int? id, int? amount)
        {
            if (id != null)
            {
                return RedirectToAction("ClientsProfile", "Protected");
            }
            return HttpNotFound();
        }

        public ActionResult TransferMoney(int? id)
        {
            if (id != null)
            {
                Account account = db.Accounts.FirstOrDefault(a => a.AccountId == id);
                if (account != null)
                {
                    if (OperationService.FundsCanBeAdded(account))
                        return View();
                }
            }
            return HttpNotFound();
        }
    }
}
