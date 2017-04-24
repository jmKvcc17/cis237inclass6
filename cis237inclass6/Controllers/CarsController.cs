using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237inclass6.Models;

namespace cis237inclass6.Controllers
{
    [Authorize] // 
    public class CarsController : Controller
    {
        private CarsJMeachumEntities db = new CarsJMeachumEntities();

        // GET: Cars
        public ActionResult Index() //******************
        {
            //Setup a variable to hold the cars data
            DbSet<Car> CarsToFilter = db.Cars;

            // Setup some strings to hold the data that might be in the session.
            // If there is nothing in the session we can still use these variables
            // as a default value
            string filterMake = "";
            string filterMin = "";
            string filterMax = "";

            

            // Define a min and max for the cylinders
            int min = 0;
            int max = 16;

            // Check to see if there is a value in the session, and if there is, assign it
            // to the variable that we setup to hold the value
            if (!string.IsNullOrWhiteSpace((string)Session["make"]))
            {
                filterMake = (string)Session["make"];
            }

            if (!string.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Int32.Parse(filterMin);
            }

            if (!string.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = Int32.Parse(filterMax);
            }

            // Do the filter on the CarsToFilter Dataset. Use the where that we used before 
            // when doing the last inclass, only this time send in more lamda expressions to
            // narrow it down further. Since we setup the default values for each of the filter
            // parameters, min, max and filterMake, we can count on this always running with no errors.
            IEnumerable<Car> filtered = CarsToFilter.Where(car => car.cylinders >= min 
                && car.cylinders <= max &&
                car.make.Contains(filterMake));

            // Convert the dataset to a list now that the query work is done on it.
            // The view is expecting a list, so we convert the database set to a list
            IEnumerable<Car> finalFiltered = filtered.ToList();

            ViewBag.filterMake = filterMake;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            // Return the view with the filtered selection of cars
            return View(finalFiltered);

            // What was originally here
            //return View(db.Cars.ToList());
        }

        // GET: Cars/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,year,make,model,type,horsepower,cylinders")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,year,make,model,type,horsepower,cylinders")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index"); // Goto a different method to show that the edit has worked
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
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

        // Mark the method as Post since it is reached from a form submit
        // Make sure to validate the antiforgeryToken too since we include it in the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            // Get the form data that we sent out of the request object.
            // the string that is used as a key to get the data matches the
            // name property of the form control
            string make = Request.Form.Get("make");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            // Now that we have the data pulled out from the request object,
            // let's put it into the session so that other methods can have access to it
            Session["make"] = make;
            Session["min"] = min;
            Session["max"] = max;

            // Redirect to the index page
            return RedirectToAction("Index");
        }

        public ActionResult JSON()
        {
            return Json(db.Cars.ToList(), JsonRequestBehavior.AllowGet); // second parameter: If user wants GET request, its okay
        }
    }
}
