using EasyBank.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyBank.Controllers
{
    public class FileController : Controller
    {
        ConnectionContext db = new ConnectionContext();
        //
        // GET: /File/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                Image photo = new Image();
                photo.Name = System.IO.Path.GetFileName(file.FileName);
                byte[] n = new byte[file.InputStream.Length];

                file.InputStream.Read(n, 0, (int)file.InputStream.Length);
                photo.ImageContent = n;
                photo.ContentType = file.ContentType;

                db.Images.Add(photo);

                db.SaveChanges();
            }
            return View();
        }

    }
}
