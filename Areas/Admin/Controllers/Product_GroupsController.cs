using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace MyEshop.Areas.Admin.Controllers
{
    public class Product_GroupsController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Admin/Product_Groups
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult ListGroups()
        {
            var product_Groups = db.Product_GroupsRepository.Get(p => p.ParentID == null);

            return PartialView(product_Groups.ToList());
        }

        // GET: Admin/Product_Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Groups product_Groups = db.Product_GroupsRepository.GetById(id);
            if (product_Groups == null)
            {
                return HttpNotFound();
            }
            return View(product_Groups);
        }

        // GET: Admin/Product_Groups/Create
        public ActionResult Create(int? id)
        {
            return PartialView(new Product_Groups()
            {
                ParentID = id
            });
        }

        // POST: Admin/Product_Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupID,GroupTitle,ParentID")] Product_Groups product_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Product_GroupsRepository.Insert(product_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(p => p.ParentID == null));
            }

            ViewBag.ParentID = new SelectList(db.Product_GroupsRepository.Get(), "GroupID", "GroupTitle", product_Groups.ParentID);
            return View(product_Groups);
        }

        // GET: Admin/Product_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_Groups product_Groups = db.Product_GroupsRepository.GetById(id);
            if (product_Groups == null)
            {
                return HttpNotFound();
            }
            return View(product_Groups);
        }

        // POST: Admin/Product_Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupID,GroupTitle,ParentID")] Product_Groups product_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Product_GroupsRepository.Update(product_Groups);
                db.Save();
                return PartialView("ListGroups", db.Product_GroupsRepository.Get(p => p.ParentID == null));
            }
            ViewBag.ParentID = new SelectList(db.Product_GroupsRepository.Get(), "GroupID", "GroupTitle", product_Groups.ParentID);
            return View(product_Groups);
        }

        // GET: Admin/Product_Groups/Delete/5
        public void Delete(int id)
        {
            var Group = db.Product_GroupsRepository.GetById(id);
            if (Group.Product_Groups1.Any())
            {
                foreach(var subGroup in db.Product_GroupsRepository.Get(g=>g.ParentID == id))
                {
                    db.Product_GroupsRepository.Delete(subGroup);
                    foreach(var minGroup in db.Product_GroupsRepository.Get(g=>g.ParentID == subGroup.GroupID))
                    {
                        db.Product_GroupsRepository.Delete(minGroup);
                    }
                }
            }
            db.Product_GroupsRepository.Delete(Group);
            db.Save();
        }

        // POST: Admin/Product_Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Product_GroupsRepository.Delete(id);
            db.Save();
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
