using OnlineShopProject.Models;
using OnlineShopProject.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineShopProject.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        UserRepository uRepo = new UserRepository();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            Users user = new Users();
            user.UEmail = email;
            user.UPassword = password;
            //var cUser = uRepo.GetByEmail(email);
            bool isValid = uRepo.returnValue(user);
            if (isValid)
            {
                FormsAuthentication.SetAuthCookie(email, false);

                var cUser = uRepo.GetBySingleUser(email, password);
                Session["Users"] = cUser;

                if (cUser.Count() > 0)
                {
                    Session["id"] = cUser.FirstOrDefault().Uid;
                    Session["Username"] = cUser.FirstOrDefault().Username;
                    Session["Email"] = cUser.FirstOrDefault().UEmail;
                    Session["Created"] = cUser.FirstOrDefault().CreatedOn;
                    Session["Modified"] = cUser.FirstOrDefault().ModifiedOn;
                    Session["role"] = cUser.FirstOrDefault().UserRole;
                    if (cUser.FirstOrDefault().UserRole == "Admin")
                    {
                        Session["role"] = "Admin";
                        return RedirectToAction("Index", "Admin",cUser);
                    }
                    else
                    {
                        Session["role"] = "Customer";
                        return RedirectToAction("Dashboard", "Customer");
                    }
                }
                else
                {
                    ViewBag.msg = "error";
                }

                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ViewBag.Error = "Username or Password is incorrect";
                return View();
            }
        }
        
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(string username, string email, string password, string Cpassword)
        {
            if(password == Cpassword)
            {
                Users user = new Users();

                user.Username = username;
                user.UEmail = email;
                user.UPassword = password;
                user.CreatedOn = DateTime.Now;
                user.UserRole = "Customer";
                uRepo.Insert(user);
                EmailConfirmation(email);
                return RedirectToAction("Login");
            }
            else
            {
                Session["error"] = "Password is not matched";
                return RedirectToAction("Registration");
            }
            
        }








        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPasswordLink()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPasswordLink(string email)
        {
            string resetCode = Guid.NewGuid().ToString();
            var verifyUrl = "/Account/ResetPassword/" + resetCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            using (var context = new QuickStoreDB())
            {
                var getUser = (from s in context.Users where s.UEmail == email select s).FirstOrDefault();
                if (getUser != null)
                {
                    getUser.ResetPasswordCode = resetCode;

                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 

                    context.Configuration.ValidateOnSaveEnabled = false;
                    context.SaveChanges();

                    var subject = "Password Reset Request";
                    var body = "Hi " + getUser.Username + ", <br/> You recently requested to reset your password for your account. Click the link below to reset it. " +

                         " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                         "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";

                    SendEmail(getUser.UEmail, body, subject);

                    ViewBag.Message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    ViewBag.Message = "User doesn't exists.";
                    return View();
                }
            }

            return View();
        }


        [NonAction]
        private void SendEmail(string emailAddress, string body, string subject)
        {

            var fromEmail = new MailAddress("quickstoreshop.bd@gmail.com", "QuickStore");
            var toEmail = new MailAddress(emailAddress);
            var fromEmailPassword = "Quick3@store"; // Replace with actual password
            string subjectpart = subject;

            string bodypart = body;

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
                Subject = subjectpart,
                Body = bodypart,
                IsBodyHtml = true
            })
                smtp.Send(message);



            /*using (MailMessage mm = new MailMessage("quickstoreshop.bd@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("quickstoreshop.bd@gmail.com", "QuickStore");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);

            }*/
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (var context = new QuickStoreDB())
            {
                var user = context.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (var context = new QuickStoreDB())
                {
                    var user = context.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //you can encrypt password here, we are not doing it
                        user.UPassword = model.NewPassword;
                        //make resetpasswordcode empty string now
                        user.ResetPasswordCode = "";
                        //to avoid validation issues, disable it
                        context.Configuration.ValidateOnSaveEnabled = false;
                        context.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }







        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Abandon();
            return RedirectToAction("Index","Customer");
        }


        [NonAction]
        public void EmailConfirmation(string email)
        {
            
            var fromEmail = new MailAddress("quickstoreshop.bd@gmail.com", "QuickStore");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "Quick3@store"; // Replace with actual password
            string subject = "Congratulations! Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your QuickStore account is" +
                " successfully created. Please visit our website to buy your necessary things" ;

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

    }
    
}