using EasyBank.Filters;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
