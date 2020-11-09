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
    public class Feature_GroupsController : Controller
    {
        UnitOfWork db = new UnitOfWork();

        // GET: Admin/Feature_Groups
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListFeature()
        {
            var Feature_Groups = db.Feature_GroupsRepository.Get(g => g.FeatureParentID == null);
            return PartialView(Feature_Groups);
        }

        // GET: Admin/Feature_Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature_Groups feature_Groups = db.Feature_GroupsRepository.GetById(id);
            if (feature_Groups == null)
            {
                return HttpNotFound();
            }
            return View(feature_Groups);
        }

        // GET: Admin/Feature_Groups/Create
        public ActionResult Create(int? id)
        {
            return PartialView(new Feature_Groups()
            {
                FeatureParentID = id
            });
        }

        // POST: Admin/Feature_Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FeatureGroupID,FeatureGroupTitle,FeatureParentID")] Feature_Groups feature_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Feature_GroupsRepository.Insert(feature_Groups);
                db.Save();
                return PartialView("ListFeature", db.Feature_GroupsRepository.Get(f => f.FeatureParentID == null));
            }

            ViewBag.FeatureParentID = new SelectList(db.Feature_GroupsRepository.Get(), "FeatureGroupID", "FeatureGroupTitle", feature_Groups.FeatureParentID);
            return View(feature_Groups);
        }

        // GET: Admin/Feature_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feature_Groups feature_Groups = db.Feature_GroupsRepository.GetById(id);
            if (feature_Groups == null)
            {
                return HttpNotFound();
            }
            return PartialView(feature_Groups);
        }

        // POST: Admin/Feature_Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeatureGroupID,FeatureGroupTitle,FeatureParentID")] Feature_Groups feature_Groups)
        {
            if (ModelState.IsValid)
            {
                db.Feature_GroupsRepository.Update(feature_Groups);
                db.Save();
                return PartialView("ListFeature", db.Feature_GroupsRepository.Get(f => f.FeatureParentID == null));

            }
            ViewBag.FeatureParentID = new SelectList(db.Feature_GroupsRepository.Get(), "FeatureGroupID", "FeatureGroupTitle", feature_Groups.FeatureParentID);
            return View(feature_Groups);
        }

        // GET: Admin/Feature_Groups/Delete/5
        public void Delete(int id)
        {
            var Group = db.Feature_GroupsRepository.GetById(id);
            if (Group.Feature_Groups1.Any())
            {
                foreach(var subGorup in db.Feature_GroupsRepository.Get(g=>g.FeatureParentID == id))
                {
                    db.Feature_GroupsRepository.Delete(subGorup);
                    foreach(var minGroup in db.Feature_GroupsRepository.Get(g=>g.FeatureParentID == subGorup.FeatureGroupID))
                    {
                        db.Feature_GroupsRepository.Delete(minGroup);
                    }
                }
            }
            db.Feature_GroupsRepository.Delete(Group);
            db.Save();
        }

        // POST: Admin/Feature_Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Feature_GroupsRepository.Delete(id);
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
