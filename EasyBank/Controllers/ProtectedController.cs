﻿using EasyBank.Filters;
using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using EasyBank.DAL;
using System.IO;
using SimpleMembershipTest.Filters;
using System.Net.Mime;

namespace EasyBank.Controllers
{
    [Culture]
    [Authorize]
    [InitializeSimpleMembership]
    public class ProtectedController : Controller
    {
        /*[Authorize(Roles="Administrator, Operator")]
        public ActionResult PreRegister(RegisterCompositeModel registerCompModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    ClientsImage photo = new ClientsImage();
                    photo.Name = System.IO.Path.GetFileName(file.FileName);
                    byte[] n = new byte[file.InputStream.Length];

                    file.InputStream.Read(n, 0, (int)file.InputStream.Length);
                    photo.ImageContent = n;
                    photo.ContentType = file.ContentType;
                    photo.PhotoType = (int)ImageType.PassportScan;
                    db.Images.Add(photo);

                    Client client = new Client();
                    client.Name = registerCompModel.Name;
                    client.Surname = registerCompModel.Surname;
                    client.PIdNumber = registerCompModel.PIdNumber;
                    client.BirthDate = registerCompModel.BirthDate;
                    client.Email = registerCompModel.Email;
                    client.RegistrationDate = DateTime.Now;
                    db.Clients.Add(client);
                    db.SaveChanges();

                    var registerModel = new RegisterModel();
                    registerModel.UserName = registerCompModel.Email;
                    registerModel.Password = registerCompModel.Password;
                    registerModel.ConfirmPassword = registerCompModel.ConfirmPassword;
                    TempData["a"] = registerModel;


                    return RedirectToAction("Register","Account", new { model = registerModel });
                }

            }
            return View();
        } */
        //
        // GET: /Client/
        private ConnectionContext db = new ConnectionContext();
        public ActionResult Index()
        {
            List<ErrorReport> errorReports = (from e in db.ErrorReports select e).ToList();
            foreach (ErrorReport errorReport in errorReports)
            {
                errorReport.Account = db.Accounts.Find(errorReport.AccountId);
            }
            return View(errorReports);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult OperatorsList(string sort, string currentFilter, string Search, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.NameSort = String.IsNullOrEmpty(sort) ? "name_desc" : "";
            ViewBag.SurnameSort = sort == "surname_desc" ? "surname_asc" : "surname_desc";
            ViewBag.EmailSort = sort == "email_desc" ? "email_asc" : "email_desc";
            ViewBag.RegistrationDateSort = sort == "registrationDate_desc" ? "registrationdate_asc" : "registrationDate_desc";

            var operators = from opr in db.Operators
                          select opr;

            string[] searchWords = null;
            if (!String.IsNullOrEmpty(Search))
            {
                searchWords = new string[2];
                searchWords = Search.Split(' ');

                if (searchWords.Length == 1)
                {
                    operators = operators.Where(c => c.Name.ToUpper().Contains(Search.ToUpper())
                                           || c.Surname.ToUpper().Contains(Search.ToUpper()));
                }
                else if (searchWords.Length == 2)
                {
                    string word1 = searchWords[0];
                    string word2 = searchWords[1];
                    operators = operators.Where(c => (c.Name.ToUpper().Contains(word1.ToUpper())
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
                    operators = operators.OrderByDescending(c => c.Name);
                    break;
                case "surname_desc":
                    operators = operators.OrderByDescending(c => c.Surname);
                    break;
                case "surname_asc":
                    operators = operators.OrderBy(c => c.Surname);
                    break;
                case "email_desc":
                    operators = operators.OrderByDescending(c => c.Email);
                    break;
                case "email_asc":
                    operators = operators.OrderBy(c => c.Email);
                    break;
                case "registrationDate_desc":
                    operators = operators.OrderByDescending(c => c.RegistrationDate);
                    break;
                case "registrationdate_asc":
                    operators = operators.OrderBy(c => c.RegistrationDate);
                    break; 
                default:
                    operators = operators.OrderBy(c => c.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(operators.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles="Operator, Administrator")]
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
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(clients.ToPagedList(pageNumber, pageSize));
        }

        //link to:
        //add account
        public ActionResult ClientsProfile(int? clientId)
        {
            var client = db.Clients.FirstOrDefault(c => c.ClientId == clientId);
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
                if (db.Clients.FirstOrDefault(c => c.PIdNumber == client.PIdNumber) != null)
                    return HttpNotFound();//Change for partial view later-----------------------!!!!!!!!!!!!!!!
                if (file != null)
                {                    
                    ClientsImage photo = new ClientsImage();
                    photo.Name = System.IO.Path.GetFileName(file.FileName);
                    byte[] n = new byte[file.InputStream.Length];

                    file.InputStream.Read(n, 0, (int)file.InputStream.Length);
                    photo.ImageContent = n;
                    photo.ContentType = file.ContentType;
                    photo.PhotoType = (int)ImageType.PassportScan;

                    db.Images.Add(photo);

                    client.RegistrationDate = DateTime.Now;
                    
                    db.Clients.Add(client);
                    if (fileIsImage(file))
                    {
                        db.SaveChanges();
                        return RedirectToAction("ClientsList");
                    }
                    ViewBag.Message = @Resources.Resource.WrongFileChoose;
                    return View();
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
        public ActionResult Capture(int? id)
        {
            var stream = Request.InputStream;
            string dump;

            using (var reader = new StreamReader(stream))
                dump = reader.ReadToEnd();

            var image = (from images in db.Images
                         where images.ClientId == id
                         where images.PhotoType == 1
                         select images).FirstOrDefault();
            if (image != null)
            {
                image.ImageContent = String_To_Bytes2(dump);
                db.Entry(image).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //    var path = Server.MapPath("~/test.jpg");
                //       System.IO.File.WriteAllBytes(path, String_To_Bytes2(dump));
                ClientsImage photo = new ClientsImage();
                photo.Name = db.Clients.Single(model => model.ClientId == id).PIdNumber;
                photo.ImageContent = String_To_Bytes2(dump);
                ContentType ct = new ContentType("image/jpeg");
                photo.ContentType = ct.MediaType;
                photo.PhotoType = (int)ImageType.ClientPhoto;
                photo.ClientId = (int)id;

                db.Images.Add(photo);
                db.SaveChanges();
            }
            return RedirectToAction("ClientsProfile", new { clientId = id });
        }
        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        public ActionResult ShowPassport(int id)
        {

            var image = (from images in db.Images
                         where images.ClientId == id
                         where images.PhotoType == 0
                         select images).FirstOrDefault();

            return View(image);
        }
        public ActionResult ShowClientPhoto(int? id)
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
        public ActionResult AddAccount(int? clientId)
        {
            var ListTypes = db.AccountTypes.ToList();
            ViewBag.Types = ListTypes;
            var ListCurrency = db.Currencies.ToList();
            ViewBag.Currencys = ListCurrency;
            var ListDeposits = (from deps in db.DepositCreditModels
                                where deps.AccountTypeId == 2
                                select deps).ToList();
            ViewBag.Deposits = ListDeposits;
            var ListCredits = (from creds in db.DepositCreditModels
                               where creds.AccountTypeId == 3
                               select creds).ToList();
            ViewBag.Credits = ListCredits;
            if (clientId != null)
            {
                ViewBag.ClientId = clientId;

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
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("ClientsProfile", new { clientId = account.ClientId });
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                return RedirectToAction("AddAccount");
            }
        }
 
        [HttpGet]
        public ActionResult ChooseBankProduct(int accountId)
        {
            var type = (int)(from accts in db.Accounts
                       where accts.AccountId == accountId
                       select accts.TypeId).FirstOrDefault();
            if (type == 2)
            {
                var deposits = (from d in db.DepositCreditModels
                                where d.AccountTypeId == 2
                                select d).ToList();
                ViewBag.AccountId = accountId;
                return View(deposits);
            }
            else if (type == 3)
            {
                var credits = (from c in db.DepositCreditModels
                               where c.AccountTypeId == 3
                               select c).ToList();

                return View(credits);   
            }
            return View();
        }
        
        [HttpPost]   
        public ActionResult ChooseBankProduct(DepositCreditModel depoCreditModel)
        {

            return RedirectToAction("ClientsProfile");
        }
        
        [Authorize(Roles = "Administrator, Operator")]
        public ActionResult CurrencyList()
        {
            var mostRecentEntries = (from currency in db.Currencies select currency).ToList();
            ViewBag.Currencies = mostRecentEntries;
            return View();
        } 

        [Authorize(Roles = "Administrator")]
        public ActionResult AddCurrency(String name)
        {
            if (name != null)
            {
                if ((from Currency in db.Currencies where Currency.CurrencyName == name select Currency).Count() == 0 && name.Length != 0)
                {
                    Currency currency = new Currency();
                    currency.CurrencyName = name;
                    db.Currencies.Add(currency);
                    db.SaveChanges();
                }
                else
                {
                    TempData["verifyAdd"] = "<script>alert('This currency is avaliable');</script>";
                }
                return RedirectToAction("CurrencyList");
            }
            else
            {
                return RedirectToAction("CurrencyList");
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult verifyDelete(int? id)
        {
            
            if (id != null)
            {
                var CurrencyDelete = (from Currency in db.Currencies
                                      where Currency.CurrencyId == id
                                      select Currency).First();
                var CurrencyAvaliable = (from Account in db.Accounts
                                         where Account.CurrencyId == CurrencyDelete.CurrencyId
                                         select Account);
                if (CurrencyAvaliable.Count()==0)
                {
                    return View(CurrencyDelete);
                }
                else
                {
                    TempData["verifyDelete"] = "<script>alert('This currency is in using');</script>";
                    return RedirectToAction("CurrencyList");
                }
            }
            else
            {
                return RedirectToAction("CurrencyList");
            }

        }

        [Authorize(Roles = "Administrator")]
        public ActionResult CurrencyDelete(int? id)
        {
            if (id != null)
            {
                var CurrencyDelete = (from Currency in db.Currencies
                                      where Currency.CurrencyId == id
                                      select Currency).First();
                try
                {
                    db.Currencies.Remove(CurrencyDelete);
                    db.SaveChanges();
                    return RedirectToAction("CurrencyList");
                }
                catch
                {
                    return View(CurrencyDelete);
                }
            }
            else
            {
                return HttpNotFound();
            }
        }

        private Boolean fileIsImage(HttpPostedFileBase file)
        {

            string fileType = file.FileName.ToString().Remove(0, file.FileName.LastIndexOf('.'));
            if (fileType == ".jpg" || fileType == ".jpeg" || fileType == ".JPG" || fileType == ".JPEG" || fileType == ".png" || fileType == ".PNG")
            {
                return true;
            }
            return false;
        }

    }
}
