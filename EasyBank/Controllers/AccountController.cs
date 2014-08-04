using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using EasyBank.Filters;
using EasyBank.Models;
using SimpleMembershipTest.Filters;
using EasyBank.DAL;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace EasyBank.Controllers
{
    [Culture]
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        ConnectionContext db = new ConnectionContext();
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            /*LoginModel newLoginModel = new LoginModel();
            newLoginModel.CapchaAmount = 3;*/
            //return View(newLoginModel);
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CaptchaMvc.Attributes.CaptchaVerify("Captcha is not valid")]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            //ModelState.Remove("CapchaAmount");
            string errorMessage = "The user name or password or capcha provided is incorrect. ";
            if (ModelState.IsValid)
            {
                if (Roles.IsUserInRole(model.UserName, "Administrator") || Roles.IsUserInRole(model.UserName, "Operator"))
                {
                    //model.CapchaAmount--;
                    //errorMessage += model.CapchaAmount.ToString() + " attempts left.";
                    ModelState.AddModelError("", errorMessage);
                    return View(model);

                }
                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("ClientsProfile", "Account");
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", errorMessage);
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [Authorize(Roles="Operator")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        // Register Clients
        [HttpPost]
        [Authorize(Roles="Operator")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterCompositeModel registerCompModel, HttpPostedFileBase file )
        {
            if (ModelState.IsValid)
            {
                 if (db.Clients.FirstOrDefault(c => c.PIdNumber == registerCompModel.Client.PIdNumber) != null)
                    return HttpNotFound();//Change for partial view later-----------------------!!!!!!!
                 if (file != null)
                 {
                     // Attempt to register the user
                     try
                     {
                         var client = new Client();
                         client.Name = registerCompModel.Client.Name;
                         client.Surname = registerCompModel.Client.Surname;
                         client.PIdNumber = registerCompModel.Client.PIdNumber;
                         client.BirthDate = registerCompModel.Client.BirthDate;
                         client.Email = registerCompModel.Client.Email;
                         client.RegistrationDate = DateTime.Now;
                         db.Clients.Add(client);

                         var model = new RegisterModel();
                         model.UserName = registerCompModel.Client.Email;
                         model.Password = registerCompModel.Password;
                         model.ConfirmPassword = registerCompModel.ConfirmPassword;
                         WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                         Roles.AddUserToRole(model.UserName, "Client");


                         ClientsImage photo = new ClientsImage();
                         if (fileIsImage(file))
                         {
                             photo.Name = System.IO.Path.GetFileName(file.FileName);
                             byte[] n = new byte[file.InputStream.Length];
                             file.InputStream.Read(n, 0, (int)file.InputStream.Length);
                             photo.ImageContent = GetCompressedImage(n);
                             photo.ContentType = file.ContentType;
                             photo.PhotoType = (int)ImageType.PassportScan;
                             photo.ClientId = client.ClientId;
                             db.Images.Add(photo);
                             db.SaveChanges();
                             return RedirectToAction("ClientsList", "Protected");
                         }
                     }
                     catch (MembershipCreateUserException e)
                     {
                         ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                     }
                 }
            }
            ViewBag.Message = @Resources.Resource.WrongFileChoose;
            // If we got this far, something failed, redisplay form
            return View(registerCompModel);
        }


        //Register Operators
        [HttpGet]
        [Authorize(Roles="Administrator")]
        public ActionResult RegisterOperator()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles="Administrator")]
        public ActionResult RegisterOperator(Operator oper)
        {
            if (ModelState.IsValid)
            {
                    // Attempt to register the Operator
                    try
                    {
                        oper.RegistrationDate = DateTime.Now;
                        db.Operators.Add(oper);
                        db.SaveChanges();

                        var model = new RegisterModel();
                        model.UserName = oper.Email;
                        model.Password = oper.Password;
                        model.ConfirmPassword = oper.Password;
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                        Roles.AddUserToRole(model.UserName, "Operator");
                        return RedirectToAction("Index", "Home");
                    }
                    catch (MembershipCreateUserException e)
                    {
                        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    }
                }
            // If we got this far, something failed, redisplay form
            return View(oper);
        }

        //Register Admin
        /*
        [AllowAnonymous]
        public ActionResult RegisterAdmin(RegisterModel admin)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the Operator
                try
                {
                    WebSecurity.CreateUserAndAccount(admin.UserName, admin.Password);
                    WebSecurity.Login(admin.UserName, admin.Password);
                    Roles.AddUserToRole(admin.UserName, "Administrator");
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            // If we got this far, something failed, redisplay form
            return View(admin);
        }
        */
        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");// ---------------------ПЕРЕКЛАД
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");// ---------------------ПЕРЕКЛАД
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
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

        public ActionResult ClientsProfile(int? id)
        {
            //Client client = new Client();
            //Client user = (from clients in db.Clients
            //               where clients.ClientId == clientId
            //               select clients).FirstOrDefault();
            string userName = WebSecurity.CurrentUserName;
            var client = (from c in db.Clients
                          where c.Email == userName
                          select c).FirstOrDefault();
           /*Client user = db.Clients.FirstOrDefault(c => c.ClientId == id.Value);
            client.ClientId = user.ClientId;
            client.BirthDate = user.BirthDate;
            client.Email = user.Email;
            client.PIdNumber = user.PIdNumber;
            client.Name = user.Name;
            client.RegistrationDate = user.RegistrationDate;
            client.Surname = user.Surname;
            client.Images = user.Images;
            client.Accounts = user.Accounts;*/
            return View(client);
        }



        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        private byte[] GetCompressedImage(byte[] originalBytes)
        {
            Size size = new Size();
            size.Width = 640;
            size.Height = 480;
            ImageFormat format = ImageFormat.Jpeg;
            using (var streamOriginal = new MemoryStream(originalBytes))
            using (var imgOriginal = Image.FromStream(streamOriginal))
            {

                //get original width and height of the incoming image
                var originalWidth = imgOriginal.Width; // 1000
                var originalHeight = imgOriginal.Height; // 800

                //get the percentage difference in size of the dimension that will change the least
                var percWidth = ((float)size.Width / (float)originalWidth); // 0.2
                var percHeight = ((float)size.Height / (float)originalHeight); // 0.25
                var percentage = Math.Max(percHeight, percWidth); // 0.25

                //get the ideal width and height for the resize (to the next whole number)
                var width = (int)Math.Max(originalWidth * percentage, size.Width); // 250
                var height = (int)Math.Max(originalHeight * percentage, size.Height); // 200

                //actually resize it
                using (var resizedBmp = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage((Image)resizedBmp))
                    {
                        graphics.InterpolationMode = InterpolationMode.Default;
                        graphics.DrawImage(imgOriginal, 0, 0, width, height);
                    }

                    //work out the coordinates of the top left pixel for cropping
                    var x = (width - size.Width) / 2; // 25
                    var y = (height - size.Height) / 2; // 0

                    //create the cropping rectangle
                    var rectangle = new Rectangle(x, y, size.Width, size.Height); // 25, 0, 200, 200

                    //crop
                    using (var croppedBmp = resizedBmp.Clone(rectangle, resizedBmp.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        //get the codec needed
                        var imgCodec = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == format.Guid);

                        //make a paramater to adjust quality
                        var codecParams = new EncoderParameters(1);

                        //reduce to quality of 80 (from range of 0 (max compression) to 100 (no compression))
                        codecParams.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

                        //save to the memorystream - convert it to an array and send it back as a byte[]
                        croppedBmp.Save(ms, imgCodec, codecParams);
                        return ms.ToArray();
                    }
                }
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

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
