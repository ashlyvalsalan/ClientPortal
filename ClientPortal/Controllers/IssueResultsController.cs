using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClientPortal.Models;

namespace ClientPortal.Controllers
{
    public class IssueResultsController : Controller
    {
        private Streemline3_1Entities db = new Streemline3_1Entities();

        // GET: IssueResults
        public ActionResult Index()
        {
            return View(db.tblIssueResults.ToList());
        }

        // GET: IssueResults/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueResult tblIssueResult = db.tblIssueResults.Find(id);
            if (tblIssueResult == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueResult);
        }

        // GET: IssueResults/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssueResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "resultID,name,description")] tblIssueResult tblIssueResult)
        {
            if (ModelState.IsValid)
            {
                db.tblIssueResults.Add(tblIssueResult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblIssueResult);
        }

        // GET: IssueResults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueResult tblIssueResult = db.tblIssueResults.Find(id);
            if (tblIssueResult == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueResult);
        }

        // POST: IssueResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "resultID,name,description")] tblIssueResult tblIssueResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblIssueResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblIssueResult);
        }

        // GET: IssueResults/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueResult tblIssueResult = db.tblIssueResults.Find(id);
            if (tblIssueResult == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueResult);
        }

        // POST: IssueResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblIssueResult tblIssueResult = db.tblIssueResults.Find(id);
            db.tblIssueResults.Remove(tblIssueResult);
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
