using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyBank.Filters;
using EasyBank.Models;

namespace EasyBank.Controllers
{
    [Culture]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        ConnectionContext db = new ConnectionContext();

        public ActionResult OperationError(int? errorCode, int? AccountId)
        {
            ErrorReport errorReport = new ErrorReport();
            ViewBag.ErrorCode = errorCode;
            errorReport.AccountId = (int)AccountId;
            Account account = db.Accounts.FirstOrDefault(a => a.AccountId == AccountId);
            return View(errorReport);
        }
        [HttpPost]
        public ActionResult OperationError(ErrorReport errorreport)
        {

            errorreport.Text = errorreport.Text + "\r\n" + ViewBag.ErrorCode;
            errorreport.ReportDate = DateTime.Now;
            errorreport.Account = db.Accounts.SingleOrDefault(m => m.AccountId == errorreport.AccountId);
            db.ErrorReports.Add(errorreport);
            db.SaveChanges();
            return View("Yes");
        }

    }
}
