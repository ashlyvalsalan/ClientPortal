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
    public class IssueTypesController : Controller
    {
        private Streemline3_1Entities db = new Streemline3_1Entities();

        // GET: IssueTypes
        public ActionResult Index()
        {
            return View(db.tblIssueTypes.ToList());
        }

        // GET: IssueTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueType tblIssueType = db.tblIssueTypes.Find(id);
            if (tblIssueType == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueType);
        }

        // GET: IssueTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssueTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "typeID,typeTitle,typeDescription,typeImage,showInFeedbackSystem,TypeIcon")] tblIssueType tblIssueType)
        {
            if (ModelState.IsValid)
            {
                db.tblIssueTypes.Add(tblIssueType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblIssueType);
        }

        // GET: IssueTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueType tblIssueType = db.tblIssueTypes.Find(id);
            if (tblIssueType == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueType);
        }

        // POST: IssueTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "typeID,typeTitle,typeDescription,typeImage,showInFeedbackSystem,TypeIcon")] tblIssueType tblIssueType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblIssueType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblIssueType);
        }

        // GET: IssueTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueType tblIssueType = db.tblIssueTypes.Find(id);
            if (tblIssueType == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueType);
        }

        // POST: IssueTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblIssueType tblIssueType = db.tblIssueTypes.Find(id);
            db.tblIssueTypes.Remove(tblIssueType);
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
