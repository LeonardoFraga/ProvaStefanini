using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;

namespace Customer.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            var userConnected = (UserSys)Session["UserConnected"];

            if (userConnected == null)
            {
                return RedirectToAction("Login", "UserSys");
            }

            return RedirectToAction("Index", "Customers");
        }

       
    }
}