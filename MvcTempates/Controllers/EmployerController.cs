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
            model.CurrentSortedColumn = "ID";
            model.SortDirection = HtmlHelperExtensions.SortDirection.Ascending;

            return View(model);
        }

        public ActionResult Refresh(EmployerSearchViewModel esvm)
        {
            string sortedColumn, currentSortedColumn;
            HtmlHelperExtensions.SortDirection sortDirection;
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
                    esvm.ResultSet = getSortedList(esvm, out sortedColumn, out currentSortedColumn, out sortDirection).ToPagedList(esvm.RequestedPage, 3);
                    break;
                case HtmlHelperExtensions.TableActions.Search:
                    esvm.RequestedPage = 1;
                    esvm.ResultSet = esvm.ResultSet.OrderBy(cc => cc.ID).ToPagedList(esvm.RequestedPage, 3);
                    break;
                case HtmlHelperExtensions.TableActions.Sort:
                    esvm.RequestedPage = 1;
                    esvm.ResultSet = getSortedList(esvm, out sortedColumn, out currentSortedColumn, out sortDirection).ToPagedList(esvm.RequestedPage, 3);
                    esvm.SortedColumn = sortedColumn;
                    esvm.CurrentSortedColumn = currentSortedColumn;
                    esvm.SortDirection = sortDirection;
                    break;
                default:
                    break;
            }
            return PartialView(esvm);
        }

        private IEnumerable<Employer> getSortedList(EmployerSearchViewModel esvm, out string sortedColumn, out string currentSortedColumn, out HtmlHelperExtensions.SortDirection sortDirection)
        {
            if (esvm.SortedColumn==null || esvm.SortedColumn =="")

            //no new sorting requested
            {
                if (esvm.CurrentSortedColumn == null || esvm.CurrentSortedColumn == "")
                //there is no current sorting
                {
                    sortedColumn = null;
                    currentSortedColumn = "ID";
                    sortDirection = HtmlHelperExtensions.SortDirection.Ascending;
                    return esvm.ResultSet.OrderBy(cc => cc.ID);
                }
                else
                //there is a current sorting
                {
                    currentSortedColumn = esvm.CurrentSortedColumn;
                    sortedColumn = null;
                    sortDirection = esvm.SortDirection;
                    if (esvm.SortDirection == HtmlHelperExtensions.SortDirection.Ascending || esvm.SortDirection == null)
                        return esvm.ResultSet.OrderBy(cc => (GetPropertyValue(cc, esvm.CurrentSortedColumn)));
                    else
                        return esvm.ResultSet.OrderByDescending(cc => (GetPropertyValue(cc, esvm.CurrentSortedColumn)));
                }

            }
            else
            //a new sorting is requested
            {
                if (esvm.SortedColumn == null || esvm.SortedColumn == "")
                //currently no sorting
                {
                    currentSortedColumn = esvm.SortedColumn;
                    sortedColumn = null;
                    sortDirection = HtmlHelperExtensions.SortDirection.Ascending;
                    return esvm.ResultSet.OrderBy(cc => (GetPropertyValue(cc, esvm.SortedColumn)));
                }
                else
                //already a sortimg
                {
                    if (esvm.SortedColumn == esvm.CurrentSortedColumn)
                    // toggle sorting asc /desc
                    {
                        currentSortedColumn = esvm.CurrentSortedColumn;
                        sortedColumn = null;
                        sortDirection = esvm.SortDirection == HtmlHelperExtensions.SortDirection.Ascending ?  HtmlHelperExtensions.SortDirection.Descending : HtmlHelperExtensions.SortDirection.Ascending;
                        if (sortDirection == HtmlHelperExtensions.SortDirection.Ascending)
                            return esvm.ResultSet.OrderBy(cc => (GetPropertyValue(cc, esvm.SortedColumn)));
                        else
                            return esvm.ResultSet.OrderByDescending(cc => (GetPropertyValue(cc, esvm.SortedColumn)));
                    }
                    else
                    //new sorting field
                    {
                        currentSortedColumn = esvm.SortedColumn;
                        sortedColumn = null;
                        sortDirection = HtmlHelperExtensions.SortDirection.Ascending;
                        return esvm.ResultSet.OrderBy(cc => (GetPropertyValue(cc, esvm.SortedColumn)));
                    }
                }
            }

        }


        /// <summary>
        /// Returns a PropertyInfo based on the property string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
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