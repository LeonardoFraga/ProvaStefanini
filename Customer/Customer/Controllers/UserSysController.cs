using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace Customer.Controllers
{
    public class UserSysController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: UserSys
        public ActionResult Index()
        {
            return View(db.UserSys.ToList());
        }

        

        // POST: UserSys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSys userSys = db.UserSys.Find(id);
            db.UserSys.Remove(userSys);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                UserSys user = db.UserSys.FirstOrDefault(x => x.Email == email && x.Password == password);

                var encryptedPassword = EncryptPassword(password);
                
                if (user == null)
                    user = db.UserSys.FirstOrDefault(x => x.Email == email && x.Password == encryptedPassword);
                
                if(user != null)
                {


                    if (user.Password != EncryptPassword(password))
                    {
                        var encryptedUserPassword = EncryptPassword(user.Password);

                        //The user password is not encrypted in DB
                        if (user.Password != encryptedUserPassword)
                        {
                            EncryptUserPasswordInDB(user);
                            user.Password = encryptedUserPassword;
                        }
                    }

                    if (string.Compare(user.Password, encryptedPassword, true) == 0)
                    {
                        if (user != null)
                        {
                            FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                            string returnUrl = Request.QueryString["ReturnUrl"];
                            Session["UserConnected"] = user;
                            if (returnUrl != null)
                                return Redirect(returnUrl);
                            else
                                return RedirectToAction("Index", "Customers");

                        }
                    }
                }

                ViewBag.Message = "The e-mail and/ or password entered is invalid.Please try again.";
                return View();
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private string EncryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                var computedHash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

                var sb = new StringBuilder();
                for (int i = 0; i < computedHash.Length; i++)
                    sb.Append(computedHash[i].ToString("x2"));

                return sb.ToString();
            }
        }

        private void EncryptUserPasswordInDB(UserSys user)
        {
            UserSys userSys = db.UserSys.Find(user.Id);
            userSys.Password = EncryptPassword(userSys.Password);
            //userSys.Password = "admin123";
            db.SaveChanges();
            }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
