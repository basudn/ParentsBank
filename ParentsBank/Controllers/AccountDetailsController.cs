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
    public class AccountDetailsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountDetails
        public async Task<ActionResult> Index()
        {
            string user = User.Identity.Name;
            List<AccountDetails> accounts = await db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower() || model.Recipient.ToLower() == user.ToLower()).ToListAsync();
            if (accounts.Count() == 0 || accounts[0].Owner.ToLower() == user.ToLower())
            {
                return View(accounts);
            }
            else
            {
                return RedirectToAction("Details", new { id = accounts[0].Id });
            }
        }

        // GET: AccountDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user = User.Identity.Name;
            AccountDetails accountDetails = await db.Accounts.FindAsync(id);
            if (accountDetails == null || (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower()))
            {
                return HttpNotFound();
            }
            if (accountDetails.Owner.ToLower() == user.ToLower())
            {
                ViewBag.Role = "Owner";
            }
            return View(accountDetails);
        }

        // GET: AccountDetails/Create
        public ActionResult Create()
        {
            List<AccountDetails> accounts = db.Accounts.Where(acct => acct.Recipient == User.Identity.Name).ToList();
            if (accounts.Count > 0)
            {
                return RedirectToAction("Details", new { id = accounts[0].Id });
            }
            return View();
        }

        // POST: AccountDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate,Balance")] AccountDetails accountDetails)
        {
            accountDetails.OpenDate = DateTime.Now;
            accountDetails.Owner = User.Identity.Name;
            ValidateAccountDetails(accountDetails);
            if (ModelState.IsValid)
            {
                db.Accounts.Add(accountDetails);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(accountDetails);
        }

        // GET: AccountDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string user = User.Identity.Name;
            AccountDetails accountDetails = await db.Accounts.FindAsync(id);
            if (accountDetails == null || (accountDetails.Owner.ToLower() != user.ToLower() && accountDetails.Recipient.ToLower() != user.ToLower()))
            {
                return HttpNotFound();
            }
            if (accountDetails.Owner.ToLower() == user.ToLower())
            {
                ViewBag.Role = "Owner";
            }
            return View(accountDetails);
        }

        // POST: AccountDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,InterestRate,Balance")] AccountDetails accountDetails)
        {
            AccountDetails storedDetails = db.Accounts.Where(acct => acct.Id == accountDetails.Id).ToList()[0];
            storedDetails.Name = accountDetails.Name;
            if (storedDetails.Owner == User.Identity.Name)
            {
                storedDetails.Recipient = accountDetails.Recipient;
                storedDetails.InterestRate = accountDetails.InterestRate;
            }
            ValidateAccountDetails(storedDetails);
            if (ModelState.IsValid)
            {
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accountDetails);
        }

        // GET: AccountDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountDetails accountDetails = await db.Accounts.FindAsync(id);
            string user = User.Identity.Name;
            if (accountDetails == null || accountDetails.Owner.ToLower() != user.ToLower())
            {
                return HttpNotFound();
            }
            else
            {
                return View(accountDetails);
            }
        }

        // POST: AccountDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AccountDetails accountDetails = await db.Accounts.FindAsync(id);
            if (accountDetails.Owner != User.Identity.Name)
            {
                return HttpNotFound();
            }
            if (accountDetails.Balance == 0)
            {
                db.Accounts.Remove(accountDetails);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("Balance", "Cannot delete account with available balance.");
                return View(accountDetails);
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

        public bool CheckAccountRecipientEmail(string recipientEmail, int? accountId)
        {
            var countQuery = db.Accounts.Where(acct => acct.Recipient.ToLower() == recipientEmail.ToLower());
            if (accountId != null)
            {
                countQuery = db.Accounts.Where(acct => acct.Recipient.ToLower() == recipientEmail.ToLower() && acct.Id != accountId);
            }
            if (countQuery.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckAccountRecipientOwner(string recipientEmail)
        {
            int count = db.Accounts.Where(acct => acct.Owner.ToLower() == recipientEmail.ToLower()).Count();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckAccountOwnerRecipient(string ownerEmail)
        {
            int count = db.Accounts.Where(acct => acct.Recipient.ToLower() == ownerEmail.ToLower()).Count();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public void ValidateAccountDetails(AccountDetails accountDetails)
        {
            if (accountDetails.Owner.ToLower().Equals(accountDetails.Recipient.ToLower()))
            {
                ModelState.AddModelError("Recipient", "Recipient email cannot be same as owner email.");
            }
            if (CheckAccountRecipientEmail(accountDetails.Recipient, accountDetails.Id))
            {
                ModelState.AddModelError("Recipient", "Recipient already has an account.");
            }
            if (CheckAccountOwnerRecipient(accountDetails.Owner))
            {
                ModelState.AddModelError("Owner", "Owner already registered as a recipient.");
            }
            if (CheckAccountRecipientOwner(accountDetails.Recipient))
            {
                ModelState.AddModelError("Recipient", "Recipient already registered as an owner.");
            }
        }
    }
}
