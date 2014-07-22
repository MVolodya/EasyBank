using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBank.Controllers
{
    public class OperationController : Controller
    {
        //
        // GET: /Operation/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddMoney()
        {
            return View();
        }

        public ActionResult TransferMoney()
        {
            return View();
        }
    }
}
