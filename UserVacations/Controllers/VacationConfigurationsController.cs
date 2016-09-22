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
    public class VacationConfigurationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VacationConfigurations
        public ActionResult Index()
        {
            return View(db.VacationConfigurations.ToList());
        }

        // GET: VacationConfigurations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationConfiguration vacationConfiguration = db.VacationConfigurations.Find(id);
            if (vacationConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(vacationConfiguration);
        }

        // GET: VacationConfigurations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VacationConfigurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserID,VacationYear,VacationDays,RemainingDays,EarnedDays")] VacationConfiguration vacationConfiguration)
        {
            if (ModelState.IsValid)
            {
                db.VacationConfigurations.Add(vacationConfiguration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vacationConfiguration);
        }

        // GET: VacationConfigurations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationConfiguration vacationConfiguration = db.VacationConfigurations.Find(id);
            if (vacationConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(vacationConfiguration);
        }

        // POST: VacationConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,VacationYear,VacationDays,RemainingDays,EarnedDays")] VacationConfiguration vacationConfiguration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacationConfiguration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vacationConfiguration);
        }

        // GET: VacationConfigurations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationConfiguration vacationConfiguration = db.VacationConfigurations.Find(id);
            if (vacationConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(vacationConfiguration);
        }

        // POST: VacationConfigurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VacationConfiguration vacationConfiguration = db.VacationConfigurations.Find(id);
            db.VacationConfigurations.Remove(vacationConfiguration);
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
