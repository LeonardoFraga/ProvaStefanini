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

namespace Customer.Controllers
{
    public class UsersController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.UserRoles);
            return View(users.ToList());
        }

       

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Users users = db.Users.Find(id);
            db.Users.Remove(users);
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
                Users user = db.Users.Include(u => u.UserRoles).FirstOrDefault(x => x.Email == email && x.Password == password);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Login, false);
                    string returnUrl = Request.QueryString["ReturnUrl"];
                    if (returnUrl != null)
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index");
                }
                ViewBag.Message = "Email e/ou senha inválidos";
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

        // GET: Users
        public List<Users> FindUser(string email,string password)
        {
            var users = db.Users.Include(u => u.UserRoles).Where(x => x.Email == email && x.Password == password);
            return users.ToList();
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
