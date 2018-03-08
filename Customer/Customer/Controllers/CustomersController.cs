using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using Customer = Customer.Models.Customer;

//using Customer = Customer.Models.Customer;

namespace Customer.Controllers
{

    public class CustomersController : Controller
    {
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Customers
        public ActionResult Index()
        {

            var userConnected = (UserSys)Session["UserConnected"];

            if (userConnected == null)
            {
                return RedirectToAction("Login", "UserSys");
            }


            return ApplyFilters();


        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Phone,GenderId,CityId,RegionId,LastPurchase,ClassificationId,UserId")] Models.Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customer.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Phone,GenderId,CityId,RegionId,LastPurchase,ClassificationId,UserId")] Models.Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Customer customer = db.Customer.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        public ActionResult ApplyFilters(string gender = "", string name = "", string city = "", string region = "", string classification = "", string seller = "", string dateMin = "", string dateMax = "")
        {

            var userConnected = (UserSys)Session["UserConnected"];

            if (userConnected == null)
            {
                return RedirectToAction("Login", "UserSys");
            }

            List<Models.Customer> customers = db.Customer.ToList();

            #region LoadFilters

            ViewBag.Regions = db.Region.ToList();
            ViewBag.Cities = db.City.ToList();
            ViewBag.Classifications = db.Classification.ToList();
            ViewBag.Genders = db.Gender.ToList();
            ViewBag.Sellers = db.UserSys.Where(x => x.UserRoleId != 1).ToList();

            #endregion

            if (!string.IsNullOrEmpty(name))
                customers = customers.Where(x => x.Name.IndexOf(name) >= 0).ToList();

            if (!string.IsNullOrEmpty(city))
                customers = customers.Where(x => x.CityId.ToString() == city).ToList();

            if (!string.IsNullOrEmpty(region))
                customers = customers.Where(x => x.RegionId.ToString() == region).ToList();

            if (!string.IsNullOrEmpty(classification))
                customers = customers.Where(x => x.ClassificationId.ToString() == classification).ToList();

            if (!string.IsNullOrEmpty(seller))
                customers = customers.Where(x => x.UserId.ToString() == seller).ToList();

            if (!string.IsNullOrEmpty(gender))
                customers = customers.Where(x => x.GenderId.ToString() == gender).ToList();

            if (!string.IsNullOrEmpty(dateMin))
            {
                //customers = customers.Where(x => int.Parse(x.LastPurchase.ToString()) >= int.Parse(dateMin)).ToList();
                DateTime date = Convert.ToDateTime(dateMin);
                customers = customers.Where(x => x.LastPurchase >= date).ToList();
            }

            if (!string.IsNullOrEmpty(dateMax))
            {
                DateTime date = Convert.ToDateTime(dateMax);
                customers = customers.Where(x => x.LastPurchase <= date).ToList();
            }

            if (userConnected.UserRoleId == 1)
            {
                ViewBag.Administrator = true;
                ViewBag.Customers = customers;
                return View(customers);
            }
            else
            {
                ViewBag.Administrator = false;
                customers = customers.Where(x => x.UserId == userConnected.Id).ToList();
                ViewBag.Customers = customers;
                return View(customers);
            }

        }

        public JsonResult GetRegions(string id)
        {

            City city = db.City.FirstOrDefault(x => x.Id.ToString() == id);

            var regions = new List<SelectListItem>();

            regions.Add(new SelectListItem() { Value = "", Text = "" });
            regions.Add(new SelectListItem() { Value = city.Region.Id.ToString(), Text = city.Region.Name });

            return Json(new SelectList(regions, "Value", "Text"));
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.Customer customer = db.Customer.Find(id);
            db.Customer.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
