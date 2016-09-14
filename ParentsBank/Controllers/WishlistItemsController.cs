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
    public class WishlistItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishlistItems
        public ActionResult Index()
        {
            var wishlistItems = db.WishlistItems.Include(w => w.Account);
            return View(wishlistItems.ToList());
        }

        // GET: WishlistItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = db.WishlistItems.Find(id);
            if (wishlistItem == null)
            {
                return HttpNotFound();
            }
            return View(wishlistItem);
        }

        // GET: WishlistItems/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner");
            return View();
        }

        // POST: WishlistItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishlistItem wishlistItem)
        {
            if (ModelState.IsValid)
            {
                db.WishlistItems.Add(wishlistItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlistItem.AccountId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = db.WishlistItems.Find(id);
            if (wishlistItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlistItem.AccountId);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishlistItem wishlistItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wishlistItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlistItem.AccountId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = db.WishlistItems.Find(id);
            if (wishlistItem == null)
            {
                return HttpNotFound();
            }
            return View(wishlistItem);
        }

        // POST: WishlistItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishlistItem wishlistItem = db.WishlistItems.Find(id);
            db.WishlistItems.Remove(wishlistItem);
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
