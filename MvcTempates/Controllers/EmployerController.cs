using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcTempates.Models;
using PagedList;

namespace MvcTempates.Controllers
{
    public class EmployerController : Controller
    {
        private EntityContext db = new EntityContext();

        //
        // GET: /Employer/

        public ActionResult Index()
        {
            var model = new EmployerSearchViewModel();
            model.ResultSet = db.Employers.OrderBy(cc => cc.ID).ToPagedList(1, 3);
            model.RequestedPage = 1;
            model.MaxSearchResult = 100;
            return View(model);
        }

        public ActionResult Refresh(EmployerSearchViewModel esvm)
        {
            if (esvm.SearchText != string.Empty && esvm.SearchText != null)
            {
                esvm.ResultSet = db.Database.SqlQuery<Employer>(string.Format(@"SELECT * FROM dbo.Employers em 
                                                                           INNER JOIN dbo.ftsEmployers('{0}') fte 
                                                                           ON em.ID = fte.[Key]", esvm.SearchText), esvm.SearchText);
            }
            else
            {
                esvm.ResultSet = db.Employers;
            }

            switch (esvm.TableAction)
            {
                case HtmlHelperExtensions.TableActions.Page:
                    break;
                case HtmlHelperExtensions.TableActions.Search:
                    esvm.RequestedPage = 1;
                    break;
                case HtmlHelperExtensions.TableActions.Sort:
                    break;
                default:
                    break;
            }
            esvm.ResultSet = esvm.ResultSet.OrderBy(cc => cc.ID).ToPagedList(esvm.RequestedPage, 3);

            return PartialView(esvm);
        }

        
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}