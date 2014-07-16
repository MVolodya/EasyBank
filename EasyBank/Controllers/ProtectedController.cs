using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBank.Controllers
{
    public class ProtectedController : Controller
    {
        //
        // GET: /Client/

        public ActionResult Index()
        {
            return View();
        }

        //volodya
        //links to: 
        //clients profile
        //add Client
        //remove client
        //edit client
        public ActionResult ClientsList()
        {
            return View();
        }

        //link to:
        //add account
        public ActionResult ClientsProfile(int? id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddClient()
        {

            return View();
        }
        
        [HttpPost]
        public ActionResult AddClient(Client client)
        {
            return RedirectToAction("/");
        }

        [HttpGet]
        public ActionResult EditClient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditClient(int? id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddAccount(int? UserId)
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAccount(int UserId)
        {
            return View();
        }

    }
}
