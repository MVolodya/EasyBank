using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyBank.Filters;

namespace EasyBank.Controllers
{
    [Culture]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult OperationError(int? errorCode)
        {
            ViewBag.ErrorCode = errorCode;
            return View();
        }

    }
}
