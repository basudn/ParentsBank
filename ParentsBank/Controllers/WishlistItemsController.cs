using System;
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
            var wishlistItems = db.WishlistItems.Include(w => w.Account).Where(w => w.Account.Owner.ToLower() == user.ToLower() || w.Account.Recipient.ToLower() == user.ToLower());
            return View(await wishlistItems.ToListAsync());
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
        public async Task<ActionResult> Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishlistItem wishlistItem)
        {
            AccountDetails accountDetails = await db.Accounts.FindAsync(wishlistItem.AccountId);
            string user = User.Identity.Name;
            if (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            wishlistItem.DateAdded = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.WishlistItems.Add(wishlistItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower()).Union(db.Accounts.Where(model => model.Recipient.ToLower() == user.ToLower())).ToList();
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
            List<AccountDetails> list = db.Accounts.Where(acct => acct.Id == wishlistItem.AccountId).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View(wishlistItem);
        }

        // POST: WishlistItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishlistItem wishlistItem)
        {
            WishlistItem storedWishlist = db.WishlistItems.Where(wish => wish.Id == wishlistItem.Id).ToList()[0];
            AccountDetails accountDetails = await db.Accounts.FindAsync(storedWishlist.AccountId);
            string user = User.Identity.Name;
            if (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            wishlistItem.AccountId = storedWishlist.AccountId;
            if (ModelState.IsValid)
            {
                db.Entry(storedWishlist).State = EntityState.Detached;
                db.Entry(wishlistItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<AccountDetails> list = db.Accounts.Where(acct => acct.Id == wishlistItem.AccountId).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
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
            if (wishlistItem == null || (wishlistItem.Account.Owner.ToLower() != user.ToLower() && wishlistItem.Account.Recipient.ToLower() != user.ToLower()))
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
            if (wishlistItem.Account.Owner.ToLower() != user.ToLower() && wishlistItem.Account.Recipient.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            if (wishlistItem.Account.Recipient.ToLower() == user.ToLower())
            {
                ModelState.AddModelError("Purchased", "Recipient cannot delete a wishlist item.");
                return View(wishlistItem);
            }
            else {
                db.WishlistItems.Remove(wishlistItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
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
