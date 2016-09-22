using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UserVacations.Models;

namespace UserVacations.Controllers
{
    public class EmployeeVacationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmployeeVacations
        public ActionResult Index()
        {
            return View(db.Vacations.ToList());
        }

        // GET: EmployeeVacations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeVacation employeeVacation = db.Vacations.Find(id);
            if (employeeVacation == null)
            {
                return HttpNotFound();
            }
            return View(employeeVacation);
        }

        // GET: EmployeeVacations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeVacations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,VacationFrom,VacationTo,StatusVacation,VacationYearTaken,VacationNote,UserID,TypeId")] EmployeeVacation employeeVacation)
        {
            if (ModelState.IsValid)
            {
                db.Vacations.Add(employeeVacation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employeeVacation);
        }

        // GET: EmployeeVacations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeVacation employeeVacation = db.Vacations.Find(id);
            if (employeeVacation == null)
            {
                return HttpNotFound();
            }
            return View(employeeVacation);
        }

        // POST: EmployeeVacations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,VacationFrom,VacationTo,StatusVacation,VacationYearTaken,VacationNote,UserID,TypeId")] EmployeeVacation employeeVacation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employeeVacation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeeVacation);
        }

        // GET: EmployeeVacations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeVacation employeeVacation = db.Vacations.Find(id);
            if (employeeVacation == null)
            {
                return HttpNotFound();
            }
            return View(employeeVacation);
        }

        // POST: EmployeeVacations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeVacation employeeVacation = db.Vacations.Find(id);
            db.Vacations.Remove(employeeVacation);
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
