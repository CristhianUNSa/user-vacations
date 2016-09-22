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
    public class MyAppUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MyAppUsers
        public ActionResult Index()
        {
            var myAppUsers = db.Users.Include(m => m.Office);
            return View(myAppUsers.ToList());
        }

        // GET: MyAppUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyAppUser myAppUser = db.Users.Find(id);
            if (myAppUser == null)
            {
                return HttpNotFound();
            }
            return View(myAppUser);
        }

        // GET: MyAppUsers/Create
        public ActionResult Create()
        {
            ViewBag.OfficeId = new SelectList(db.Offices, "ID", "Name");
            return View();
        }

        // POST: MyAppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OfficeId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] MyAppUser myAppUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(myAppUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OfficeId = new SelectList(db.Offices, "ID", "Name", myAppUser.OfficeId);
            return View(myAppUser);
        }

        // GET: MyAppUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyAppUser myAppUser = db.Users.Find(id);
            if (myAppUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.OfficeId = new SelectList(db.Offices, "ID", "Name", myAppUser.OfficeId);
            return View(myAppUser);
        }

        // POST: MyAppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OfficeId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] MyAppUser myAppUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(myAppUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OfficeId = new SelectList(db.Offices, "ID", "Name", myAppUser.OfficeId);
            return View(myAppUser);
        }

        // GET: MyAppUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyAppUser myAppUser = db.Users.Find(id);
            if (myAppUser == null)
            {
                return HttpNotFound();
            }
            return View(myAppUser);
        }

        // POST: MyAppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MyAppUser myAppUser = db.Users.Find(id);
            db.Users.Remove(myAppUser);
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
