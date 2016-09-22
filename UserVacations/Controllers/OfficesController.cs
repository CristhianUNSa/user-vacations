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
    public class OfficesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Offices
        public ActionResult Index()
        {
            return View(db.Offices.Include(o => o.VacationTypes).ToList());
        }

        // GET: Offices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Office office = db.Offices.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        // GET: Offices/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Offices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Office office)
        {
            if (ModelState.IsValid)
            {
                db.Offices.Add(office);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(office);
        }

        // GET: Offices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Office office = db.Offices
                .Include(o => o.VacationTypes)
                .Where(o => o.ID == id)
                .SingleOrDefault();
            PopulateAssignedVacationTypes(office);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        private void PopulateAssignedVacationTypes(Office office)
        {
            var allVacationTypes = db.VacationTypes;
            var officeVacTypes = new HashSet<int>(office.VacationTypes.Select(vt => vt.ID));
            var viewModel = new List<AssignedOfficeData>();
            foreach (var vacationType in allVacationTypes)
            {
                viewModel.Add(new AssignedOfficeData
                {
                    VacationTypeId = vacationType.ID,
                    VacationTypeName = vacationType.TypeName,
                    Assigned = officeVacTypes.Contains(vacationType.ID)
                });
            }
            ViewBag.VacationTypes = viewModel;
        }

        // POST: Offices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name")] Office office, string[] selectedVacationTypes)
        {
            var officeToUpdate = db.Offices
                .Include(o => o.VacationTypes)
                .Where(o => o.ID == office.ID)
                .Single();
            if (TryUpdateModel(officeToUpdate, "",
                new string[] { "Name" }))
            {
                try 
	            {	        
		            UpdateOfficeVacationTypes(selectedVacationTypes, officeToUpdate);
                    db.Entry(officeToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
	            }
	            catch (Exception)
	            {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
	            }
            }
            PopulateAssignedVacationTypes(office);
            return View(office);
        }

        private void UpdateOfficeVacationTypes(string[] selectedVacationTypes, Office office)
        {
            if (selectedVacationTypes == null)
            {
                office.VacationTypes = new List<VacationType>();
                return;
            }

            var selectedVacationTypesHS = new HashSet<string>(selectedVacationTypes);
            var officeVacationTypes = new HashSet<int>
                (office.VacationTypes.Select(vt => vt.ID));
            foreach (var vacationType in db.VacationTypes)
            {
                if (selectedVacationTypesHS.Contains(vacationType.ID.ToString()))
                {
                    if (!officeVacationTypes.Contains(vacationType.ID))
                    {
                        office.VacationTypes.Add(vacationType);
                    }
                }
                else
                {
                    if (officeVacationTypes.Contains(vacationType.ID))
                    {
                        office.VacationTypes.Remove(vacationType);
                    }
                }
            }
        }

        // GET: Offices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Office office = db.Offices.Find(id);
            if (office == null)
            {
                return HttpNotFound();
            }
            return View(office);
        }

        // POST: Offices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Office office = db.Offices.Find(id);
            db.Offices.Remove(office);
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
