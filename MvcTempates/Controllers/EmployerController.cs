using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTempates.Models;

namespace MvcTempates.Controllers
{
    public class EmployerController : Controller
    {
        private EntityContext db = new EntityContext();

        //
        // GET: /Employer/

        public ActionResult Index()
        {
            return View(db.Employers.ToList());
        }

        //
        // GET: /Employer/Details/5

        public ActionResult Details(int id = 0)
        {
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        //
        // GET: /Employer/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Employer/Create

        [HttpPost]
        public ActionResult Create(Employer employer)
        {
            if (ModelState.IsValid)
            {
                db.Employers.Add(employer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employer);
        }

        //
        // GET: /Employer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        //
        // POST: /Employer/Edit/5

        [HttpPost]
        public ActionResult Edit(Employer employer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employer);
        }

        //
        // GET: /Employer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Employer employer = db.Employers.Find(id);
            if (employer == null)
            {
                return HttpNotFound();
            }
            return View(employer);
        }

        //
        // POST: /Employer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Employer employer = db.Employers.Find(id);
            db.Employers.Remove(employer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult Search(string fulltext)
        //{
        //}


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}