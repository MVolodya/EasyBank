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
    public class HomeController : Controller
    {
        ConnectionContext db = new ConnectionContext();
        /*public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            Console.WriteLine("Works?");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";                   //-----------------------------Can be deleted

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
        public ActionResult Index()
        {
            //Example
            //returns NULL if client not found
            //CHECK FOR NULL
            //ALWAYS CHECK FOR NULL
            //!!!!!
            //List<Account>accounts = db.Clients.FirstOrDefault(c => c.Surname == "Symonenko").Accounts.ToList();
            return View();
        }

        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsoluteUri;
            // Список культур
            List<string> cultures = new List<string>() { "uk-UA", "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "en";
            }
            // Сохраняем выбранную культуру в куки
            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
                cookie.Value = lang;   // если куки уже установлено, то обновляем значение
            else
            {

                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }
    }
}
