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
        private ConnectionContext db = new ConnectionContext();
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
            return View(db.Clients.ToList());
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
            if (client != null && client.Name != null && client.PIdNumber != null && client.BirthDate != null && client.Email != null && client.RegistrationDate != null)
            {
                client.RegistrationDate = DateTime.Now;
                db.Clients.Add(client);
                db.SaveChanges();
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                RedirectToAction("/");
            }
            return RedirectToAction("/");
        }

        [HttpGet]
        public ActionResult EditClient(int? id)
        {
            Client client=null;
            if (id!=0)
                client = db.Clients.FirstOrDefault(c=>c.ClientId==id);
            if(client != null)
            {
                return View();     
            }
            else 
            {
                ViewBag.Message = "No client with this Id";
                return RedirectToAction("/ListUsers");
            }
        }

        [HttpPost]
        public ActionResult EditClient(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return View();
            }
            else
            {
                ViewBag.Message = "OOps.. Something wrong with data";
                return RedirectToAction("/EditClient");
            }
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
