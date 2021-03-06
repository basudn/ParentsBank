﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParentsBank.Models;

namespace ParentsBank.Controllers
{
    [Authorize]
    public class WishlistItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishlistItems
        public async Task<ActionResult> Index()
        {
            string user = User.Identity.Name;
            var wishlistQuery = db.WishlistItems.Include(w => w.Account).Where(w => w.Account.Owner.ToLower() == user.ToLower() || w.Account.Recipient.ToLower() == user.ToLower());
            List<WishlistItem> wishlistItems = await wishlistQuery.ToListAsync();
            int countAfford = 0;
            int countCompleted = 0;
            foreach (WishlistItem item in wishlistItems)
            {
                if (item.Purchased)
                {
                    countCompleted++;
                }
                else if (item.Cost <= (item.Account.Balance + item.Account.CalculateInterestEarnedInCurrentYear()))
                {
                    countAfford++;
                }
            }
            ViewBag.CompletedItems = countCompleted;
            ViewBag.AffordableItems = countAfford;
            if (wishlistItems.Count > 0 && wishlistItems[0].Account.Owner.ToLower() == user.ToLower())
            {
                ViewBag.Role = "Owner";
            }
            return View("Index", wishlistItems);
        }

        // GET: WishlistItems
        public async Task<ActionResult> AccountWishlist(int? id)
        {
            string user = User.Identity.Name;
            var wishlistQuery = db.WishlistItems.Include(w => w.Account).Where(w => w.Account.Owner.ToLower() == user.ToLower() || w.Account.Recipient.ToLower() == user.ToLower());
            if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                AccountDetails account = await db.Accounts.FindAsync(id);
                if (account == null || (account.Owner.ToLower() != user.ToLower() && account.Recipient.ToLower() != user.ToLower()))
                {
                    return HttpNotFound();
                }
                ViewBag.AccountId = id;
                ViewBag.AccountName = account.Name;
                wishlistQuery = wishlistQuery.Where(w => w.Account.Id == id);
            }
            List<WishlistItem> wishlistItems = await wishlistQuery.OrderBy(w => w.DateAdded).ToListAsync();
            int countAfford = 0;
            int countCompleted = 0;
            foreach (WishlistItem item in wishlistItems)
            {
                if (item.Purchased)
                {
                    countCompleted++;
                }
                else if (item.Cost <= (item.Account.Balance + item.Account.CalculateInterestEarnedInCurrentYear()))
                {
                    countAfford++;
                }
            }
            ViewBag.CompletedItems = countCompleted;
            ViewBag.AffordableItems = countAfford;
            if (wishlistItems.Count > 0 && wishlistItems[0].Account.Owner.ToLower() == user.ToLower())
            {
                ViewBag.Role = "Owner";
            }
            return View("Index", wishlistItems);
        }

        public async Task<ActionResult> Search(string itemName, string itemMinPrice, string itemMaxPrice, string itemDescription)
        {
            string user = User.Identity.Name;
            bool searchCompleted = false;
            var wishlistQuery = db.WishlistItems.Include(w => w.Account).Where(w => w.Account.Owner.ToLower() == user.ToLower() || w.Account.Recipient.ToLower() == user.ToLower());
            if (!string.IsNullOrWhiteSpace(itemName))
            {
                wishlistQuery = wishlistQuery.Where(w => w.Name.ToLower().Contains(itemName.ToLower()));
                searchCompleted = true;
            }
            if (!string.IsNullOrWhiteSpace(itemMinPrice))
            {
                try
                {
                    double min = double.Parse(itemMinPrice);
                    wishlistQuery = wishlistQuery.Where(w => w.Cost >= min);
                    searchCompleted = true;
                }
                catch (Exception e)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
                }
            }
            if (!string.IsNullOrWhiteSpace(itemMaxPrice))
            {
                try
                {
                    double max = double.Parse(itemMaxPrice);
                    wishlistQuery = wishlistQuery.Where(w => w.Cost <= max);
                    searchCompleted = true;
                }
                catch (Exception e)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
                }
            }
            if (!string.IsNullOrWhiteSpace(itemDescription))
            {
                wishlistQuery = wishlistQuery.Where(w => w.Description.ToLower().Contains(itemDescription.ToLower()));
                searchCompleted = true;
            }
            if (searchCompleted)
            {
                return View(await wishlistQuery.ToListAsync());
            }
            else
            {
                return View(new List<WishlistItem>());
            }
        }

        // GET: WishlistItems/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = await db.WishlistItems.FindAsync(id);
            string user = User.Identity.Name;
            if (wishlistItem == null || (wishlistItem.Account.Owner.ToLower() != user.ToLower() && wishlistItem.Account.Recipient.ToLower() != user.ToLower()))
            {
                return HttpNotFound();
            }
            return View(wishlistItem);
        }

        // GET: WishlistItems/Create
        public ActionResult Create()
        {
            string user = User.Identity.Name;
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower() || model.Recipient.ToLower() == user.ToLower()).ToList();
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "AccountDetails");
            }
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View();
        }

        // POST: WishlistItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AccountId,DateAdded,Name,Cost,Description,Link")] WishlistItem wishlistItem)
        {
            AccountDetails accountDetails = await db.Accounts.FindAsync(wishlistItem.AccountId);
            string user = User.Identity.Name;
            if (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            wishlistItem.DateAdded = DateTime.Today;
            if (ModelState.IsValid)
            {
                wishlistItem.Purchased = false;
                db.WishlistItems.Add(wishlistItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower() || model.Recipient.ToLower() == user.ToLower()).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, wishlistItem.AccountId);
            return View(wishlistItem);
        }

        // GET: WishlistItems/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = await db.WishlistItems.FindAsync(id);
            string user = User.Identity.Name;
            if (wishlistItem == null || (wishlistItem.Account.Owner.ToLower() != user.ToLower() && wishlistItem.Account.Recipient.ToLower() != user.ToLower()))
            {
                return HttpNotFound();
            }
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower() || model.Recipient.ToLower() == user.ToLower()).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AccountId,DateAdded,Name,Cost,Description,Link,Purchased")] WishlistItem wishlistItem)
        {
            WishlistItem storedWishlist = await db.WishlistItems.FindAsync(wishlistItem.Id);
            AccountDetails accountDetails = await db.Accounts.FindAsync(storedWishlist.AccountId);
            string user = User.Identity.Name;
            if (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                db.Entry(storedWishlist).State = EntityState.Detached;
                db.Entry(wishlistItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower() || model.Recipient.ToLower() == user.ToLower()).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            wishlistItem.Account = accountDetails;
            return View(wishlistItem);
        }

        // GET: WishlistItems/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishlistItem wishlistItem = await db.WishlistItems.FindAsync(id);
            string user = User.Identity.Name;
            if (wishlistItem == null || wishlistItem.Account.Owner.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            return View(wishlistItem);
        }

        // POST: WishlistItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WishlistItem wishlistItem = await db.WishlistItems.FindAsync(id);
            string user = User.Identity.Name;
            if (wishlistItem == null || wishlistItem.Account.Owner.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            db.WishlistItems.Remove(wishlistItem);
            await db.SaveChangesAsync();
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

        public List<SelectListItem> PopulateAccountSelectItems(List<AccountDetails> list, int? accountId)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();
            foreach (AccountDetails account in list)
            {
                SelectListItem item = new SelectListItem();
                item.Text = account.Id + " [" + account.Name + "]";
                item.Value = account.Id.ToString();
                if (accountId == account.Id)
                {
                    item.Selected = true;
                }
                selectList.Add(item);
            }
            return selectList;
        }
    }
}
