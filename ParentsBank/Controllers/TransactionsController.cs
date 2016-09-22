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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public async Task<ActionResult> Index()
        {
            string user = User.Identity.Name;
            var transactions = db.Transactions.Include(t => t.Account).Where(t => t.Account.Owner.ToLower() == user.ToLower()).Union(db.Transactions.Include(t => t.Account).Where(t => t.Account.Recipient.ToLower() == user.ToLower()));
            return View(await transactions.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            string user = User.Identity.Name;
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower()).Union(db.Accounts.Where(model => model.Recipient.ToLower() == user.ToLower())).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            AccountDetails accountDetails = await db.Accounts.FindAsync(transaction.AccountId);
            accountDetails.Balance += transaction.Amount;
            if (accountDetails.Balance < 0)
            {
                ModelState.AddModelError("","Insufficient balance for transaction.");
            }
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.Entry(accountDetails).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            string user = User.Identity.Name;
            List<AccountDetails> list = db.Accounts.Where(model => model.Owner.ToLower() == user.ToLower()).Union(db.Accounts.Where(model => model.Recipient.ToLower() == user.ToLower())).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            string user = User.Identity.Name;
            List<AccountDetails> list = db.Accounts.Where(acct => acct.Id == transaction.AccountId).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            string user = User.Identity.Name;
            List<AccountDetails> list = db.Accounts.Where(acct => acct.Id == transaction.AccountId).ToList();
            ViewBag.AccountId = PopulateAccountSelectItems(list, null);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            db.Transactions.Remove(transaction);
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
                item.Text = account.Owner.ToLower() + ", " + account.Recipient.ToLower();
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