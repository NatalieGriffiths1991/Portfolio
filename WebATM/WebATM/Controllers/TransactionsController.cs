using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebATM.Models;

namespace WebATM.Controllers
{
    public class TransactionsController : Controller
    {
        private dbEntities db = new dbEntities();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.AtmAccount).Include(t => t.TransType);
            return View(transactions.ToList());
        }
        //GET: Transactions/5 
        public ActionResult TransactionIndex(int? id)
        {
            var transactions = db.Transactions.Include(t => t.AtmAccount).Include(t => t.TransType).Where(tr => tr.AtmAccountId == id);
            return View("Index", transactions.ToList());
        }
        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }
        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber");
            ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType");
            return View();
        }
        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TransAmount,TransDate,TransTypeId,AtmAccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber", transaction.AtmAccountId);
            ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType", transaction.TransTypeId);
            return View(transaction);
        }
        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber", transaction.AtmAccountId);
            ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType", transaction.TransTypeId);
            return View(transaction);
        }
        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TransAmount,TransDate,TransTypeId,AtmAccountId")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber", transaction.AtmAccountId);
            ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType", transaction.TransTypeId);
            return View(transaction);
        }
        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }
        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
        // GET: Transactions/QuickWithdrawal
        public ActionResult QuickWithdrawal(int? id)
        {
            AtmAccount accounts = db.AtmAccounts.Find(id);
            ViewBag.AtmAccountId = accounts.AccountNumber;
            ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType");
            if (accounts == null)
            {
                return new HttpNotFoundResult();
            }
            var balance = accounts.AccountBalance;
            if (balance < 100)
            {
                return Content("Insufficient funds available");
            }
            else
            {
                balance = balance - 100;
                accounts.AccountBalance = balance;
                Transaction transaction = new Transaction();
                transaction.TransAmount = 100;
                transaction.TransDate = DateTime.Now;
                transaction.TransTypeId = 1;
                transaction.AtmAccountId = accounts.Id;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        // GET: Transactions/Withdrawal
        public ActionResult Withdrawal(int? id)
        {
            AtmAccount accounts = db.AtmAccounts.Find(id);
            ViewBag.AtmAccountId = accounts.AccountNumber;
            return View(); 
        }
        // POST: Transactions/Withdrawal
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdrawal([Bind(Include = "Id,TransAmount,TransDate,TransTypeId,AtmAccountId")]Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                AtmAccount accounts = db.AtmAccounts.Find(transaction.Id);
                ViewBag.AtmAccountId = accounts.AccountNumber;
                ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType");
                if (accounts == null)
                {
                    return new HttpNotFoundResult();
                }
                var balance = accounts.AccountBalance;
                if (balance < transaction.TransAmount)
                {
                    return Content("Insufficient funds available");
                }
                else
                {
                    balance = balance - transaction.TransAmount;
                    accounts.AccountBalance = balance;
                    transaction.TransTypeId = 3;
                    transaction.TransDate = DateTime.Now;
                    transaction.AtmAccountId = transaction.Id;
                    ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber", transaction.AtmAccountId);
                    ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType", transaction.TransTypeId);
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Transactions");
                }
            }
            return Content("Insufficient funds available");
        }
        // GET: Transactions/Deposit
        public ActionResult Deposit(int? id)
        {
            AtmAccount accounts = db.AtmAccounts.Find(id);
            ViewBag.AtmAccountId = accounts.AccountNumber;
            return View();
        }
        // POST: Transactions/Deposit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "Id,TransAmount,TransDate,TransTypeId,AtmAccountId")]Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                AtmAccount accounts = db.AtmAccounts.Find(transaction.Id);
                ViewBag.AtmAccountId = accounts.AccountNumber;
                ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType");
                if (accounts == null)
                {
                    return new HttpNotFoundResult();
                }
                    var balance = accounts.AccountBalance; //Sets account balance
                    balance = balance + transaction.TransAmount;
                    accounts.AccountBalance = balance;
                    transaction.TransTypeId = 2; //2 is deposit in the transaction type table
                    transaction.TransDate = DateTime.Now;
                    transaction.AtmAccountId = transaction.Id;
                    ViewBag.AtmAccountId = new SelectList(db.AtmAccounts, "Id", "AccountNumber", transaction.AtmAccountId);
                    ViewBag.TransTypeId = new SelectList(db.TransTypes, "Id", "TransactionType", transaction.TransTypeId);
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    return RedirectToAction("Index","Transactions");
            }
            return Content("Insufficient funds available");
        }
        
    }
}