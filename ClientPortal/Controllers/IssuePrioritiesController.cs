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
    public class IssuePrioritiesController : Controller
    {
        private Streemline3_1Entities db = new Streemline3_1Entities();

        // GET: IssuePriorities
        public ActionResult Index()
        {
            return View(db.tblIssuePriorities.ToList());
        }

        // GET: IssuePriorities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssuePriority tblIssuePriority = db.tblIssuePriorities.Find(id);
            if (tblIssuePriority == null)
            {
                return HttpNotFound();
            }
            return View(tblIssuePriority);
        }

        // GET: IssuePriorities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IssuePriorities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "priorityID,priorityTitle,priorityDescription,priorityIcon,priorityIcon2,SortOrder")] tblIssuePriority tblIssuePriority)
        {
            if (ModelState.IsValid)
            {
                db.tblIssuePriorities.Add(tblIssuePriority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblIssuePriority);
        }

        // GET: IssuePriorities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssuePriority tblIssuePriority = db.tblIssuePriorities.Find(id);
            if (tblIssuePriority == null)
            {
                return HttpNotFound();
            }
            return View(tblIssuePriority);
        }

        // POST: IssuePriorities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "priorityID,priorityTitle,priorityDescription,priorityIcon,priorityIcon2,SortOrder")] tblIssuePriority tblIssuePriority)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblIssuePriority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblIssuePriority);
        }

        // GET: IssuePriorities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblIssuePriority tblIssuePriority = db.tblIssuePriorities.Find(id);
            if (tblIssuePriority == null)
            {
                return HttpNotFound();
            }
            return View(tblIssuePriority);
        }

        // POST: IssuePriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblIssuePriority tblIssuePriority = db.tblIssuePriorities.Find(id);
            db.tblIssuePriorities.Remove(tblIssuePriority);
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
