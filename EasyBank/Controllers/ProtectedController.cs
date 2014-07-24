using EasyBank.Filters;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using EasyBank.DAL;
using System.IO;

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

        public ActionResult ClientsList(string sort, string currentFilter, string Search, int? page)
        {
            
            ViewBag.CurrentSort = sort;
            ViewBag.NameSort = String.IsNullOrEmpty(sort) ? "name_desc" : "";
            ViewBag.SurnameSort = sort == "surname_desc" ? "surname_asc" : "surname_desc";
            ViewBag.PIdNumberSort = sort == "pidnumber_desc" ? "pidnumber_asc" : "pidnumber_desc";
            ViewBag.BirthDateSort = sort == "birthDate_desc" ? "birthdate_asc" : "birthDate_desc";
            ViewBag.EmailSort = sort == "email_desc" ? "email_asc" : "email_desc";
            ViewBag.RegistrationDateSort = sort == "registrationDate_desc" ? "registrationdate_asc" : "registrationDate_desc";

            var clients = from c in db.Clients
                      select c;

            string[] searchWords = null;
            if (!String.IsNullOrEmpty(Search))
            {
                searchWords = new string[2];
                searchWords = Search.Split(' ');

                if (searchWords.Length == 1)
                {
                    clients = clients.Where(c => c.Name.ToUpper().Contains(Search.ToUpper())
                                           || c.Surname.ToUpper().Contains(Search.ToUpper())
                                           || c.PIdNumber.ToUpper().Contains(Search.ToUpper()));
                }
                else if (searchWords.Length == 2)
                {
                    string word1 = searchWords[0];
                    string word2 = searchWords[1]; 
                    clients = clients.Where(c => (c.Name.ToUpper().Contains(word1.ToUpper())
                                           && c.Surname.ToUpper().Contains(word2.ToUpper()))
                                           || (c.Name.ToUpper().Contains(word2.ToUpper())
                                           && c.Surname.ToUpper().Contains(word1.ToUpper())));              
                }
                page = 1;
            }
            else
            {
                Search = currentFilter;
            }

            ViewBag.CurrentFilter = Search;

            switch (sort)
            {
                case "name_desc":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                case "surname_desc":
                    clients = clients.OrderByDescending(c => c.Surname);
                    break;
                case "surname_asc":
                    clients = clients.OrderBy(c => c.Surname);
                    break;
                case "pidnumber_desc":
                    clients = clients.OrderByDescending(c => c.PIdNumber);
                    break;
                case "pidnumber_asc":
                    clients = clients.OrderBy(c => c.PIdNumber);
                    break;
                case "birthDate_desc":
                    clients = clients.OrderByDescending(c => c.BirthDate);
                    break;
                case "birthdate_asc":
                    clients = clients.OrderBy(c => c.BirthDate);
                    break;
                case "email_desc":
                    clients = clients.OrderByDescending(c => c.Email);
                    break;
                case "email_asc":
                    clients = clients.OrderBy(c => c.Email);
                    break;
                case "registrationDate_desc":
                    clients = clients.OrderByDescending(c => c.RegistrationDate);
                    break;
                case "registrationdate_asc":
                    clients = clients.OrderBy(c => c.RegistrationDate);
                    break;
                default:
                    clients = clients.OrderBy(c => c.Name);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(clients.ToPagedList(pageNumber, pageSize));
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
        public ActionResult AddClient(Client client, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    Image photo = new Image();
                    photo.Name = System.IO.Path.GetFileName(file.FileName);
                    byte[] n = new byte[file.InputStream.Length];

                    file.InputStream.Read(n, 0, (int)file.InputStream.Length);
                    photo.ImageContent = n;
                    photo.ContentType = file.ContentType;
                    photo.PhotoType = (int)ImageType.PassportScan;

                    db.Images.Add(photo);

                    client.RegistrationDate = DateTime.Now;

                    db.Clients.Add(client);
                    db.SaveChanges();
                    return RedirectToAction("ClientsList");
                }
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                return RedirectToAction("AddClient");
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditClient(int? id)
        {
            Client client = null;
            if (id != null)
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
        public ActionResult AddPhoto(int? id)
        {
            Client client = null;
            if (id != null)
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
        public ActionResult AddPhoto(int id)
        {
            if (ModelState.IsValid)
            {
                if (Request != null && Request.Files != null && Request.Files.Count > 0)
                {
                    var clientPhoto = Request.Files["file"];

                    Image photo = new Image();
                    photo.Name = System.IO.Path.GetFileName(clientPhoto.FileName);
                    byte[] n = new byte[clientPhoto.InputStream.Length];

                    clientPhoto.InputStream.Read(n, 0, (int)clientPhoto.InputStream.Length);
                    photo.ImageContent = n;
                    photo.ContentType = clientPhoto.ContentType;
                    photo.ClientId = id;
                    photo.PhotoType = (int)ImageType.ClientPhoto;
                    db.Images.Add(photo);

                    db.SaveChanges();
                    return RedirectToAction("ClientsList");
                }
            }

            ViewBag.Message = "OOps.. Something wrong with data";
            return RedirectToAction("EditClient");
        }

        public ActionResult ShowPassport(int id)
        {

            var image = (from images in db.Images
                         where images.ClientId == id
                         where images.PhotoType == 0
                         select images).FirstOrDefault();

            return View(image);
        }
        public ActionResult ShowClientPhoto(int id)
        {
            var image = (from images in db.Images
                         where images.ClientId == id
                         where images.PhotoType == 1
                         select images).FirstOrDefault();
            if (image != null)
            {
                return PartialView(image);
            }

            return PartialView();

        }

        [HttpGet]
        public ActionResult AddAccount(int? UserId)
        {
            var ListTypes = db.AccountTypes.ToList();
            ViewBag.Types = ListTypes;
            var ListCurrency = db.Currencies.ToList();
            ViewBag.Currencys = ListCurrency;
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
