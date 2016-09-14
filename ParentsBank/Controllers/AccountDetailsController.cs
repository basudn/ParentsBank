using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParentsBank.Models;

namespace ParentsBank.Controllers
{
    public class AccountDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountDetails
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: AccountDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountDetails accountDetails = db.Accounts.Find(id);
            if (accountDetails == null)
            {
                return HttpNotFound();
            }
            return View(accountDetails);
        }

        // GET: AccountDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate")] AccountDetails accountDetails)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(accountDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accountDetails);
        }

        // GET: AccountDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountDetails accountDetails = db.Accounts.Find(id);
            if (accountDetails == null)
            {
                return HttpNotFound();
            }
            return View(accountDetails);
        }

        // POST: AccountDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate")] AccountDetails accountDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountDetails);
        }

        // GET: AccountDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountDetails accountDetails = db.Accounts.Find(id);
            if (accountDetails == null)
            {
                return HttpNotFound();
            }
            return View(accountDetails);
        }

        // POST: AccountDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountDetails accountDetails = db.Accounts.Find(id);
            db.Accounts.Remove(accountDetails);
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
