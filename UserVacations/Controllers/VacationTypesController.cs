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
    public class VacationTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VacationTypes
        public ActionResult Index()
        {
            return View(db.VacationTypes.ToList());
        }

        // GET: VacationTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationType vacationType = db.VacationTypes.Find(id);
            if (vacationType == null)
            {
                return HttpNotFound();
            }
            return View(vacationType);
        }

        // GET: VacationTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VacationTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TypeName,ColumnRelated,Value,Action")] VacationType vacationType)
        {
            if (ModelState.IsValid)
            {
                db.VacationTypes.Add(vacationType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vacationType);
        }

        // GET: VacationTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationType vacationType = db.VacationTypes.Find(id);
            if (vacationType == null)
            {
                return HttpNotFound();
            }
            return View(vacationType);
        }

        // POST: VacationTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TypeName,ColumnRelated,Value,Action")] VacationType vacationType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacationType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vacationType);
        }

        // GET: VacationTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationType vacationType = db.VacationTypes.Find(id);
            if (vacationType == null)
            {
                return HttpNotFound();
            }
            return View(vacationType);
        }

        // POST: VacationTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VacationType vacationType = db.VacationTypes.Find(id);
            db.VacationTypes.Remove(vacationType);
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
