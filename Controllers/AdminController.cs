using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopProject.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class AdminController : Controller
    {
        BlogRepository bRpo = new BlogRepository();

        UserRepository uRepo = new UserRepository();
        ContactRepository cRepo = new ContactRepository();
        ShippingRepository sRepo = new ShippingRepository();

        [Authorize(Roles ="Admin")]
        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            Session["role"] = "Admin";
            Session["count"] = uRepo.GetAll().Count().ToString();
            Session["order"] = sRepo.GetAll().Count().ToString();
            var profit = sRepo.GetAll();
            double count = 0;
            foreach(var item in profit)
            {
                count += Convert.ToDouble(item.AmountPaid);
            }
            Session["Profit"] = count;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetMailList()
        {
            return View(cRepo.GetAll());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult TableBlog()
        {
            return View(bRpo.GetAll());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddBlog()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreatedBlog(Blogging b, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/I_BlogImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            var img = "~/I_BlogImg/" + pic;
            b.BlogImage = img;
            b.Author = "Admin";
            b.PostedOn = DateTime.Now;
            bRpo.Insert(b);
            return RedirectToAction("TableBlog");
        }

        [AllowAnonymous]
        [HttpGet]
        //profile
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Settings(FormCollection fc)
        {
           /* Users u = new Users();
            u.Username = fc["username"];
            var cUser = uRepo.GetBySingleUser(Session["email"]);
            uRepo.Update()*/
            return View();
        }

      [HttpGet]
        //blog edit
        public ActionResult BlogEdit(int id)
        {
            return View(bRpo.Get(id));
        }
        [HttpPost]
        public ActionResult BlogEdit(Blogging b, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/I_BlogImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            var img = "~/I_BlogImg/" + pic;
            b.BlogImage = file != null ? img : img;
            bRpo.Update(b);
            return RedirectToAction("Index");
        }

        //news send
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult SendNews()
        {
            return View();
        }

        [HttpPost]
        //Newsletter
        public ActionResult SendNews(FormCollection fc)
        {
            NewslettetRepository nRpo = new NewslettetRepository();
            var emailList = nRpo.getAllSubscriber();
            var msg = fc["Message"];

            /*mail*/
/*            using (var smtp = new SmtpClient())
            {
                for (int h = 0; h < emailList.Length; h++)
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

                    var message = new MailMessage();
                    message.To.Add(new MailAddress(emailList[h]));
                    message.Subject = "I'm interested in your project";
                    message.Body = string.Format(body, "QuickStore", "quickstoreshop.bd@gmail.com", msg);
                    message.IsBodyHtml = true;



                    smtp.Send(message);
                }
            }
*/
             for (int h = 0; h < emailList.Length; h++)
             {
                 var address = emailList[h];
                 var fromEmail = new MailAddress("quickstoreshop.bd@gmail.com", "QuickStore");
                 var toEmail = new MailAddress(address);
                 var fromEmailPassword = "Quick3@store"; // Replace with actual password
                 string subject = "Hello subscriber! New Product Added";

                 string body = msg;
                 var smtp = new SmtpClient
                 {
                     Host = "smtp.gmail.com",
                     Port = 587,
                     EnableSsl = true,
                     DeliveryMethod = SmtpDeliveryMethod.Network,
                     UseDefaultCredentials = false,
                     Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                 };

                 using (var message = new MailMessage(fromEmail, toEmail)
                 {
                     Subject = subject,
                     Body = body,
                     IsBodyHtml = true
                 })
                     smtp.Send(message);
             }

            /*endmail*/


            return RedirectToAction("Index","Admin");
        }




    }

}