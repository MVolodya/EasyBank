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
using SimpleMembershipTest.Filters;
using System.Net.Mime;
using EasyBank.Services;
using System.Web.Security;

namespace EasyBank.Controllers
{
    [Culture]
    [Authorize]
    [InitializeSimpleMembership]
    public class ProtectedController : Controller
    {
        //
        // GET: /Client/
        private ConnectionContext db = new ConnectionContext();
        public ActionResult Index()
        {
            return View();
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

        [Authorize(Roles = "Operator, Administrator")]
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
        [Authorize(Roles = "Operator, Administrator, Client")]
        public ActionResult ClientsProfile(int? clientId, string sort)
        {
            if (clientId == null) return HttpNotFound();
            var client = db.Clients.FirstOrDefault(c => c.ClientId == clientId);

            ViewBag.CurrentSort = sort;
            ViewBag.ExpirationDateSort = String.IsNullOrEmpty(sort) ? "expDat_desc" : "";
            ViewBag.AccountNumberSort = sort == "accNum_desc" ? "accNum_asc" : "accNum_desc";
            ViewBag.AmountSort = sort == "amount_desc" ? "amount_asc" : "amount_desc";
            ViewBag.CurrencySort = sort == "currency_desc" ? "currency_asc" : "currency_desc";
            ViewBag.TypeSort = sort == "type_desc" ? "type_asc" : "type_desc";
            ViewBag.StatusSort = sort == "status_desc" ? "status_asc" : "status_desc";

            var accounts = from a in db.Accounts 
                           where a.ClientId == clientId
                           select a;

            switch (sort)
            {
                case "expDat_desc":
                    accounts = accounts.OrderByDescending(a => a.ExpirationDate);
                    break;
                case "accNum_desc":
                    accounts = accounts.OrderByDescending(a => a.AccountNumber);
                    break;
                case "accNum_asc":
                    accounts = accounts.OrderBy(a => a.AccountNumber);
                    break;
                case "amount_desc":
                    accounts = accounts.OrderByDescending(a => a.AvailableAmount);
                    break;
                case "amount_asc":
                    accounts = accounts.OrderBy(a => a.AvailableAmount);
                    break;
                case "currency_desc":
                    accounts = accounts.OrderByDescending(a => a.CurrencyId);
                    break;
                case "currency_asc":
                    accounts = accounts.OrderBy(a => a.CurrencyId);
                    break;
                case "type_desc":
                    accounts = accounts.OrderByDescending(a => a.TypeId);
                    break;
                case "type_asc":
                    accounts = accounts.OrderBy(a => a.TypeId);
                    break;
                case "status_desc":
                    accounts = accounts.OrderByDescending(a => a.StatusId);
                    break;
                case "status_asc":
                    accounts = accounts.OrderBy(a => a.StatusId);
                    break;
                default:
                    accounts = accounts.OrderBy(a => a.ExpirationDate);
                    break;
            }
            client.Accounts = accounts.ToList();
            return View(client);
        }


        [HttpGet]
        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult AddClient()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Operator, Administrator")]
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
        [Authorize(Roles = "Operator, Administrator")]
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
        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult EditClient(Client client)
        {
            if (ModelState.IsValid)
            {
                if (client.IsOnlineUser == true)
                {
                    if (client.IsAlreadyRegistered == true)
                    {
                        mailservice ms = new mailservice("easybankbionic@gmail.com", client.Email, "Відновлення доступу Easy Bank!", "Вам відкрито доступ до особистого кабінету\n\nЗ повагою Адміністрація банку");
                    }
                    if (client.IsAlreadyRegistered == false)
                    {
                        mailservice ms = new mailservice("easybankbionic@gmail.com", client.Email, Resources.Resource.Message1,
                        Resources.Resource.Message2 + " " + client.Name + " " + client.Surname + "!\n\n\n" + Resources.Resource.Message3 + "\n\n" + Resources.Resource.Message4 + " " + Resources.Resource.Message5 + "\n\n" +
                        Resources.Resource.Message6 + " " + Resources.Resource.Message7 + " " + 
                        "\n" + Resources.Resource.Message8 + "\n\n" +
                        Resources.Resource.Message9 + " " + client.InitialPassword + "\n\n" + Resources.Resource.Message10);
                        client.IsAlreadyRegistered = true;
                    }
                }
                if (client.IsOnlineUser == false)
                {
                    if (client.IsAlreadyRegistered == true)
                    {
                        mailservice ms = new mailservice("easybankbionic@gmail.com", client.Email, "Обмеження доступу Easy Bank!",
                        "Доступ до особистого кабінету обмежено");
                    }
                }
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


        [Authorize(Roles = "Operator, Administrator")]
        [HttpGet]
        public ActionResult EditCurrency(int? id)
        {
            Currency currency = null;
            if (id != null)
                currency = db.Currencies.FirstOrDefault(c => c.CurrencyId == id);
            if (currency != null)
            {
                return View(currency);
            }
            else
            {
                ViewBag.Message = "No currency with this Id";
                return RedirectToAction("CurrencyList");
            }
        }

        [HttpPost]
        public ActionResult EditCurrency(Currency currency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(currency).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CurrencyList");
            }
            else
            {
                ViewBag.Message = "OOps.. Something wrong with data";
                return RedirectToAction("CurrencyList");
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

        [Authorize(Roles = "Operator, Administrator")]
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
        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult AddDepositAccount(int? clientId)
        {
            var ListCurrency = db.Currencies.ToList();
            ViewBag.Currencys = ListCurrency;
            var ListDeposits = (from deps in db.DepositCreditModels
                                where deps.AccountTypeId == 2
                                select deps).ToList();
            ViewBag.Deposits = ListDeposits;
            if (clientId != null)
            {
                ViewBag.ClientId = clientId;

                return View();
            }
            else return HttpNotFound();
        }

        [HttpGet]
        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult AddCreditAccount(int? clientId)
        {
            var ListCurrency = db.Currencies.ToList();
            ViewBag.Currencys = ListCurrency;
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

        [HttpGet]
        [Authorize(Roles = "Operator, Administrator")]
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
        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult AddAccount(Account account)
        {
            account.Currency = db.Currencies.FirstOrDefault(c => c.CurrencyId == account.CurrencyId);
            if (account.Amount == 0) account.Amount = 0;
            account.Interest = 0;
            account.LastInterestAdded = DateTime.Now;
            account.OpenDate = DateTime.Now;
            account.StatusId = 1;
            account.AvailableAmount = account.Amount;
            var bankAccount = (from bankAcc in db.BankAccounts
                               where bankAcc.CurrencyName == account.Currency.CurrencyName
                               select bankAcc).FirstOrDefault();
            if (account.TypeId == 3)
            {
                var creditModel = (from credit in db.DepositCreditModels
                                   where credit.DepositCreditModelID == account.DepositCreditModelID
                                   select credit).FirstOrDefault();
                account.Interest = Math.Round(((decimal)creditModel.Duration / 12 * creditModel.InterestRate / 100) * account.Amount,2);
                account.AvailableAmount = account.Amount + account.Interest;
                account.LastInterestAdded = DateTime.Now.AddMonths(creditModel.Duration);
                bankAccount.Amount -= account.Amount;
                bankAccount.Amount += account.Interest;

            }
            else if (account.TypeId !=3)
            {
                bankAccount.Amount += account.Amount;
            }
            if(account.TypeId !=1){
            var duration = (from prods in db.DepositCreditModels
                            where prods.DepositCreditModelID == account.DepositCreditModelID
                            select prods.Duration).FirstOrDefault();
            account.ExpirationDate = DateTime.Now.AddMonths(duration);
            }

            // Normal status is default
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.Entry(bankAccount).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ClientsProfile", new { clientId = account.ClientId });
            }
            else
            {
                ViewBag.Message = "Problem with inputted data";
                return RedirectToAction("AddAccount");
            }
        }

        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult AddAccountPartial(int? clientId)
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

                return PartialView();
            }
            else return HttpNotFound();
            return PartialView();
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
            var mostRecentEntries = (from currency in db.Currencies 
                                     where currency.CurrencyName != "UAH"
                                     select currency).ToList();
            ViewBag.Currencies = mostRecentEntries;
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult AddCurrency(Currency currency)
        {
            if (ModelState.IsValid && currency.SaleRate > currency.PurchaseRate)
            {
                if ((from Currency in db.Currencies where Currency.CurrencyName == currency.CurrencyName select Currency).Count() == 0
                    && currency.CurrencyName.Length != 0 && currency.PurchaseRate > 0 && currency.SaleRate > 0)
                {
                    BankAccount ba = new BankAccount();

                    ba.CurrencyName = currency.CurrencyName;

                    db.Currencies.Add(currency);
                    db.BankAccounts.Add(ba);
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
                if (CurrencyAvaliable.Count() == 0)
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

        public ActionResult AccountHistory(int? id, string sort, int? page)
        {
            if (id != null)
            {
                ViewBag.CurrentSort = sort;
                ViewBag.SortDate = String.IsNullOrEmpty(sort) ? "Date_asc" : "";
                var operationHistory = from operation in db.OperationHistory 
                                       where operation.FromAccountId == id || operation.ToAccountId == id
                                       select operation;
                switch (sort)
                {
                    case "Date_asc":
                        operationHistory = operationHistory.OrderBy(operation => operation.Date);
                        break;
                    default:
                        operationHistory = operationHistory.OrderByDescending(operation => operation.Date);
                        break;
                }
                ViewBag.ClientsCardId = id;
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(operationHistory.ToPagedList(pageNumber, pageSize));
            }
            else return new HttpNotFoundResult();
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult ErrorReports()
        {
            List<ErrorReport> errorReports = (from e in db.ErrorReports select e).ToList();
            foreach (ErrorReport errorReport in errorReports)
            {
                errorReport.Account = db.Accounts.Find(errorReport.AccountId);
            }
            return View(errorReports);
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

        [Authorize(Roles = "Client")]
        public ActionResult Freeze(int? id, int? clientId)
        {
            var clientAccount = (from accounts in db.Accounts
                                 where accounts.AccountId == id
                                 select accounts).Single();
            clientAccount.AccountStatus = db.AccountStatuses.FirstOrDefault(a => a.StatusName == "Frozen");
            db.Entry(clientAccount).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ClientsProfile", clientAccount.Client);
        }

        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult Block(int? id, int? clientId)
        {
            var clientAccount = (from accounts in db.Accounts
                                 where accounts.AccountId == id
                                 select accounts).Single();
            var Client = (from clients in db.Clients
                          where clients.ClientId == clientId
                          select clients).Single();
            clientAccount.AccountStatus = db.AccountStatuses.FirstOrDefault(a => a.StatusName == "Blocked");
            db.Entry(clientAccount).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ClientsProfile", Client);
        }

        [Authorize(Roles = "Operator, Administrator")]
        public ActionResult SetToNormal(int? id, int? clientId)
        {
            var clientAccount = (from accounts in db.Accounts
                                 where accounts.AccountId == id
                                 select accounts).Single();
            var Client = (from clients in db.Clients
                          where clients.ClientId == clientId
                          select clients).Single();
            clientAccount.AccountStatus = db.AccountStatuses.FirstOrDefault(a => a.StatusName == "Normal");
            db.Entry(clientAccount).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ClientsProfile", Client);
        }
    }
}
