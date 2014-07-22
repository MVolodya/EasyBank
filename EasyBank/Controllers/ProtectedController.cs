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

        public ViewResult ClientsList(string sort, string currentFilter, string Search, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.NameSort = String.IsNullOrEmpty(sort) ? "Name" : "";
            ViewBag.SurnameSort = String.IsNullOrEmpty(sort) ? "Surname" : "";
            ViewBag.PIdNumberSort = String.IsNullOrEmpty(sort) ? "PIdNumber" : "";
            ViewBag.BirthDateSort = sort == "BirthDate" ? "birthdate" : "BirthDate";
            ViewBag.EmailSort = String.IsNullOrEmpty(sort) ? "Email" : "";
            ViewBag.RegistrationDateSort = sort == "RegistrationDate" ? "registrationdate" : "RegistrationDate";

            if (Search != null)
            {
                page = 1;
            }
            else
            {
                Search = currentFilter;
            }

            ViewBag.CurrentFilter = Search;

            var clients = from c in db.Clients
                          select c;
            if (!String.IsNullOrEmpty(Search))
            {
                clients = clients.Where(c => c.Name.ToUpper().Contains(Search.ToUpper())
                                       || c.Surname.ToUpper().Contains(Search.ToUpper())
                                       || c.PIdNumber.ToUpper().Contains(Search.ToUpper())
                                       || c.BirthDate.ToString().Contains(Search.ToString())
                                       || c.RegistrationDate.ToString().Contains(Search.ToString()));
            }
            switch (sort)
            {
                case "Name":
                    clients = clients.OrderByDescending(c => c.Name);
                    break;
                case "Surname":
                    clients = clients.OrderByDescending(c => c.Surname);
                    break;
                case "PIdNumber":
                    clients = clients.OrderByDescending(c => c.PIdNumber);
                    break;
                case "BirthDate":
                    clients = clients.OrderBy(c => c.BirthDate);
                    break;
                case "birthdate":
                    clients = clients.OrderByDescending(c => c.BirthDate);
                    break;
                case "Email":
                    clients = clients.OrderByDescending(c => c.Email);
                    break;
                case "RegistrationDate":
                    clients = clients.OrderBy(c => c.RegistrationDate);
                    break;
                case "registrationdate":
                    clients = clients.OrderByDescending(c => c.RegistrationDate);
                    break;
                default:
                    clients = clients.OrderBy(c => c.Surname);
                    break;
            }
            int pageSize = 20;
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
        public ActionResult AddPhoto(/*HttpPostedFileBase clientPhoto */ int id)
        {

               // var client = db.Clients.FirstOrDefault(c => c.ClientId == id);
                
            
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
                    
                    //db.Entry(client).State = System.Data.Entity.EntityState.Modified;
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
                         select images.ImageContent).FirstOrDefault();
            var stream = new MemoryStream(image.ToArray());
            return new FileStreamResult(stream, "image/jpeg");
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
