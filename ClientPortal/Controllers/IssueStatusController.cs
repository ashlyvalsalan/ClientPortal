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
    public class IssueStatusController : Controller
    {
        private Streemline3_1Entities db = new Streemline3_1Entities();

        // GET: IssueStatus
        public ActionResult Index()
        {
            return View(db.tblIssueStatus.ToList());
        }

        // GET: IssueStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueStatu tblIssueStatu = db.tblIssueStatus.Find(id);
            if (tblIssueStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueStatu);
        }

        // GET: IssueStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssueStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "statusID,statusTitle,statusDescription,isActive")] tblIssueStatu tblIssueStatu)
        {
            if (ModelState.IsValid)
            {
                db.tblIssueStatus.Add(tblIssueStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblIssueStatu);
        }

        // GET: IssueStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueStatu tblIssueStatu = db.tblIssueStatus.Find(id);
            if (tblIssueStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueStatu);
        }

        // POST: IssueStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "statusID,statusTitle,statusDescription,isActive")] tblIssueStatu tblIssueStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblIssueStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblIssueStatu);
        }

        // GET: IssueStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssueStatu tblIssueStatu = db.tblIssueStatus.Find(id);
            if (tblIssueStatu == null)
            {
                return HttpNotFound();
            }
            return View(tblIssueStatu);
        }

        // POST: IssueStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblIssueStatu tblIssueStatu = db.tblIssueStatus.Find(id);
            db.tblIssueStatus.Remove(tblIssueStatu);
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
