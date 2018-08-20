using Microsoft.AspNet.Identity;
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
    [Authorize]
    public class AtmAccountsController : Controller
    {
        private dbEntities db = new dbEntities();

        // GET: AtmAccounts
        public ActionResult Index()
        {
            //var atmAccounts = db.AtmAccounts.Include(a => a.AccType);
            //return View(atmAccounts.ToList());
            var userId = User.Identity.GetUserId();
            ViewModels.AtmAccountVm atmVm = new ViewModels.AtmAccountVm();
            atmVm.AtmAccountList = db.AtmAccounts.Where(a => a.AppUserId == userId).ToList();
            atmVm.UserName = User.Identity.GetUserName();
            return View(atmVm);
        }

 

    // GET: AtmAccounts/Details/5
    public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtmAccount atmAccount = db.AtmAccounts.Find(id);
            if (atmAccount == null)
            {
                return HttpNotFound();
            }
            return View(atmAccount);
        }

        // GET: AtmAccounts/Create
        public ActionResult Create()
        {
            ViewBag.AccTypeId = new SelectList(db.AccTypes, "Id", "AccountType");
            return View();
        }

        // POST: AtmAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountNumber,AccountBalance,AppUserId,AccTypeId")] AtmAccount atmAccount)
        {
            if (ModelState.IsValid)
            {
                atmAccount.AppUserId = User.Identity.GetUserId();
                db.AtmAccounts.Add(atmAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccTypeId = new SelectList(db.AccTypes, "Id", "AccountType", atmAccount.AccTypeId);
            return View(atmAccount);
        }

        // GET: AtmAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtmAccount atmAccount = db.AtmAccounts.Find(id);
            if (atmAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccTypeId = new SelectList(db.AccTypes, "Id", "AccountType", atmAccount.AccTypeId);
            return View(atmAccount);
        }

        // POST: AtmAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountNumber,AccountBalance,AppUserId,AccTypeId")] AtmAccount atmAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(atmAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccTypeId = new SelectList(db.AccTypes, "Id", "AccountType", atmAccount.AccTypeId);
            return View(atmAccount);
        }

        // GET: AtmAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AtmAccount atmAccount = db.AtmAccounts.Find(id);
            if (atmAccount == null)
            {
                return HttpNotFound();
            }
            return View(atmAccount);
        }

        // POST: AtmAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AtmAccount atmAccount = db.AtmAccounts.Find(id);
            db.AtmAccounts.Remove(atmAccount);
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
