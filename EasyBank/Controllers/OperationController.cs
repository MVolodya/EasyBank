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
        [HttpGet]
        public ActionResult AddMoney(int? Id)
        {
            ConnectionContext db = new ConnectionContext();
            //Account account = db.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            //if (account != null)
                return View();
            //else return HttpNotFound();
        }
        [HttpPost]
        public ActionResult AddMoney(int accountId, int amount)
        {
            return RedirectToAction("ClientsProfile", "Protected"/*, new { clientId = client.ClientId }*/);
        }
    }
}
