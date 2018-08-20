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
    public class WithdrawController : Controller
    {
        public decimal currencyAmount;
        private DbEntities db = new DbEntities();
        // GET: Withdraw
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: Withdraw/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        // GET: Withdraw/Create
        public ActionResult Create()
        {
            ViewBag.GetUser = HttpContext.User.Identity.Name;
            return View();
        }
        public ActionResult WithdrawAmount(string AccountNumber, string AccountPinNumber, decimal FundsToWithdraw, bool USD, bool EUR, bool JPY, bool CNY, bool RUB, bool HKD)
        {
            if (db.Accounts.Where(a => a.AccountNumber == AccountNumber).Any()) //Checks to see if account number exists in the database
            {
                //Select record that has matching account number and pinnumber and check if funds are available
                var query = from a in db.Accounts where a.AccountNumber.Contains(AccountNumber) && a.AccountPinNumber.Contains(AccountPinNumber) && (a.AccountBalance >= FundsToWithdraw) select a; 
                if (query == null) //There is no account number with that matching pin number and or the funds are not available, display error
                {
                    ViewBag.Error("The pin number didn't match or there were not enough available funds. Please check your details and try again.");
                }
               else //The account number matches that pin number
                {
                    if (ModelState.IsValid)
                    {
                        Account account_i = new Account();
                        Currency currency = new Currency();
                        if (USD != false)
                        {
                            currencyAmount = (decimal)1.37;//If USD FundsToWithdraw * 1.3700
                        }
                        if (EUR != false)
                        {
                            currencyAmount = (decimal)1.13;//If EUR FundsToWithdraw * 1.1300
                        }
                        if (JPY != false)
                        {
                            currencyAmount = (decimal)152.45;//If JPY FundsToWithdraw * 152.4600
                        }
                        if (CNY != false)
                        {
                            currencyAmount = (decimal)1.37;//If CNY FundsToWithdraw * 1.3700
                        }
                        if (RUB != false)
                        {
                            currencyAmount = (decimal)1.37;//If RUB FundsToWithdraw * 1.3700
                        }
                        if (HKD != false)
                        {
                            currencyAmount = (decimal)1.37;//Else If HKD FundsToWithdraw * 1.3700
                        }
                        var query2 = from a in db.Accounts where a.AccountNumber.Contains(AccountNumber) && a.AccountPinNumber.Contains(AccountPinNumber) && (a.AccountBalance >= FundsToWithdraw) select a;
                        if (query2 != null)
                        {
                            account_i.AccountBalance = (account_i.AccountBalance - FundsToWithdraw);
                            FundsToWithdraw = (FundsToWithdraw*currencyAmount);//If USD FundsToWithdraw * 1.3700 etc
                            Transaction transaction = new Transaction();
                            transaction.TransactionAmount = FundsToWithdraw;
                            transaction.TransactionGetUser = HttpContext.User.Identity.Name;
                            transaction.TransactionType = "Withdraw";
                            transaction.TransactionTime = DateTime.Now;
                            db.SaveChanges();
                            return RedirectToAction("SuccessView");
                        }
                        else
                        {
                            ViewBag.Error("Not enough funds");
                        }
                    }
                }
            }
            else
            {
                ViewBag.Error("AccountNumber does not exist");
            }
            return View(db);
        }
        // GET: Withdraw/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        // POST: Withdraw/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountID,AccountNumber,AccountType,AccountPinNumber,AccountBalance,GetUser")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Withdraw/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Withdraw/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
