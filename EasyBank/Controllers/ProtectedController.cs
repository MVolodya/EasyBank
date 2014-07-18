﻿using EasyBank.Filters;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBank.Controllers
{
    [Culture]
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
        public ActionResult ClientsList(string sortOrder, string SearchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            var clients = from c in db.Clients
                          select c;
            if (!String.IsNullOrEmpty(SearchString))
            {
                clients = clients.Where(c => c.Name.ToUpper().Contains(SearchString.ToUpper())
                                       || c.Surname.ToUpper().Contains(SearchString.ToUpper())
                                       || c.PIdNumber.ToUpper().Contains(SearchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                default:
                    clients = clients.OrderBy(c => c.Name);
                    break;
            }
            return View(clients);
        }

        //link to:
        //add account
        public ActionResult ClientsProfile(int? id)
        {
            var client = db.Clients.FirstOrDefault(c => c.ClientId == id);
            return View(client);
        }

        [HttpGet]
        public ActionResult AddClient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddClient(Client client)
        {
            if (ModelState.IsValid)
            {
                client.RegistrationDate = DateTime.Now;
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("ClientsList");
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                return RedirectToAction("AddClient");
            }
        }

        [HttpGet]
        public ActionResult EditClient(int? id)
        {
            Client client = null;
            if (id != 0)
                client = db.Clients.FirstOrDefault(c => c.ClientId == id);
            if (client != null)
            {
                return View(client);
            }
            else
            {
                ViewBag.Message = "No client with this Id";
                return RedirectToAction("ClientsList");
            }
        }

        [HttpPost]
        public ActionResult EditClient(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClientsList");
            }
            else
            {
                ViewBag.Message = "OOps.. Something wrong with data";
                return RedirectToAction("EditClient");
            }
        }

        [HttpGet]
        public ActionResult AddAccount(int? UserId)
        {
            if (UserId != null)
            {
                ViewBag.ClientId = UserId;
                return View();
            }
            else return HttpNotFound();
        }

        [HttpPost]
        public ActionResult AddAccount(Account account)
        {
            account.StatusId = 1;
            // Normal status is default
            if (ModelState.IsValid)
            {
                account.ExpirationDate = DateTime.Now;

                db.Accounts.Add(account);

                db.SaveChanges();
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                RedirectToAction("/");
            }
            return RedirectToAction("/");
        }

    }
}
