using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyBank.Filters;
using EasyBank.Models;
using System.Net.Mail;
using System.Net;

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
        public ActionResult OperationError(ErrorReport errorReport)
        {

            errorReport.Text = errorReport.Text + "\r\n" + ViewBag.ErrorCode;
            errorReport.ReportDate = DateTime.Now;
            errorReport.Account = db.Accounts.SingleOrDefault(m => m.AccountId == errorReport.AccountId);
            db.ErrorReports.Add(errorReport);
            db.SaveChanges();
            return RedirectToAction("Account/ClientsProfile");
        }
    }
}
