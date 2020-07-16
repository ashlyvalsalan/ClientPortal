using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace ClientPortal.Controllers
{
    public class IssueCategoriesController : Controller
    {
        private Streemline3_1Entities db = new Streemline3_1Entities();

        // GET: IssueCategories
        public ActionResult Index()
        {
            return View(db.tblIssueCategories.ToList());
        }

        // GET: IssueCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueCategory tblIssueCategory = db.tblIssueCategories.Find(id);
            if (tblIssueCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueCategory);
        }

        // GET: IssueCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssueCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "categoryID,categoryTitle,categoryDescription")] tblIssueCategory tblIssueCategory)
        {
            if (ModelState.IsValid)
            {
                db.tblIssueCategories.Add(tblIssueCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblIssueCategory);
        }

        // GET: IssueCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueCategory tblIssueCategory = db.tblIssueCategories.Find(id);
            if (tblIssueCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueCategory);
        }

        // POST: IssueCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "categoryID,categoryTitle,categoryDescription")] tblIssueCategory tblIssueCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblIssueCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblIssueCategory);
        }

        // GET: IssueCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueCategory tblIssueCategory = db.tblIssueCategories.Find(id);
            if (tblIssueCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueCategory);
        }

        // POST: IssueCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblIssueCategory tblIssueCategory = db.tblIssueCategories.Find(id);
            db.tblIssueCategories.Remove(tblIssueCategory);
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
