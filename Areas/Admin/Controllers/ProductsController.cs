using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace MyEshop.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        UnitOfWork db = new UnitOfWork();

        // GET: Admin/Products
        public ActionResult Index()
        {
            return View(db.ProductsRepository.Get());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.ProductsRepository.GetById(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.Groups = db.Product_GroupsRepository.Get();
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,Title,ShortDescription,Text,Price,PriceOff,ImageName,CreateDate")] Products products , List<int> SelectedGroups , HttpPostedFileBase ImageProduct , string tags)
        {
            if (ModelState.IsValid)
            {
                if(SelectedGroups == null)
                {
                    ViewBag.ErrorSelectedGroup = true;
                    ViewBag.Groups = db.Product_GroupsRepository.Get();
                    return View(products);
                }
                products.ImageName = "images.Jpg";
                if(ImageProduct != null && ImageProduct.IsImage())
                {
                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(ImageProduct.FileName);
                    ImageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/ProductImages/" + products.ImageName),
                        Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                }
                products.CreateDate = DateTime.Now;
                db.ProductsRepository.Insert(products);
                foreach(var SelectedGroup in SelectedGroups)
                {
                    db.Product_Selected_GroupsRepository.Insert(new Product_Selected_Groups()
                    {
                        ProductID = products.ProductID,
                        GroupID = SelectedGroup
                    });
                }
                if (!string.IsNullOrEmpty(tags))
                {
                    string[] Tag = tags.Split(',');
                    foreach(string t in Tag)
                    {
                        db.Product_TagsRepository.Insert(new Product_Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }
                db.Save();
                return RedirectToAction("Index");
            }

            return View(products);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
         
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.ProductsRepository.GetById(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedGroups = products.Product_Selected_Groups.ToList();
            ViewBag.Groups = db.Product_GroupsRepository.Get();
            ViewBag.Tags = string.Join(",", products.Product_Tags.Select(t => t.Tag).ToList());
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,Title,ShortDescription,Text,Price,PriceOff,ImageName,CreateDate")] Products products, List<int> SelectedGroups, HttpPostedFileBase ImageProduct, string tags)
        {
            if (ModelState.IsValid)
            {
                if(SelectedGroups == null)
                {
                    ViewBag.SelectedGroups = products.Product_Selected_Groups.ToList();
                    ViewBag.Groups = db.Product_GroupsRepository.Get();
                    ViewBag.Tags = tags;
                    return View(products);
                }
                if(ImageProduct != null && ImageProduct.IsImage())
                {
                    if(products.ImageName != "images.Jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                        System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
                    }
                    products.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(ImageProduct.FileName);
                    ImageProduct.SaveAs(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Images/productImages/" + products.ImageName),
                    Server.MapPath("/Images/productImages/Thumb/" + products.ImageName));
                }
                db.ProductsRepository.Update(products);
                db.Product_TagsRepository.Get(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_TagsRepository.Delete(t));
                if (!string.IsNullOrEmpty(tags))
                {
                    string[] Tag = tags.Split(',');
                    foreach(string t in Tag)
                    {
                        db.Product_TagsRepository.Insert(new Product_Tags()
                        {
                            ProductID = products.ProductID,
                            Tag = t.Trim()
                        });
                    }
                }
                db.Product_Selected_GroupsRepository.Get(g => g.ProductID == products.ProductID).ToList().ForEach(g => db.Product_Selected_GroupsRepository.Delete(g));
                foreach (int SelectedGroup in SelectedGroups)
                {
                    db.Product_Selected_GroupsRepository.Insert(new Product_Selected_Groups()
                    {
                        ProductID = products.ProductID,
                        GroupID = SelectedGroup
                    });
                }
                db.Save();
                return RedirectToAction("Index");
            }
            ViewBag.SelectedGroups = products.Product_Selected_Groups.ToList();
            ViewBag.Groups = db.Product_GroupsRepository.Get();
            ViewBag.Tags = tags;
            return View(products);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.ProductsRepository.GetById(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var products = db.ProductsRepository.GetById(id);
            db.Product_Selected_GroupsRepository.Get(g => g.ProductID == products.ProductID).ToList().ForEach(g => db.Product_Selected_GroupsRepository.Delete(g));
            db.Product_TagsRepository.Get(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_TagsRepository.Delete(t));
            db.Product_FeaturesRepository.Get(f => f.ProductID == products.ProductID).ToList().ForEach(f => db.Product_FeaturesRepository.Delete(f));
            var gallery = db.Product_GalleryiesRepository.Get(p => p.ProductID == id);
            foreach(var galle in gallery)
            {
                System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + galle.ImageName));
                System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + galle.ImageName));
                db.Product_GalleryiesRepository.Delete(galle);
            }
            if(products.ImageName != "images.Jpg")
            {
                System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + products.ImageName));
                System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + products.ImageName));
            }
            db.ProductsRepository.Delete(id);
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

        public ActionResult Gallery(int id)
        {
            ViewBag.Galleries = db.Product_GalleryiesRepository.Get(p => p.ProductID == id);
            return View(new Product_Galleryies()
            {
                ProductID = id
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gallery(Product_Galleryies galleries , HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid && imgUp != null)
            {
                galleries.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imgUp.FileName);
                imgUp.SaveAs(Server.MapPath("/Images/ProductImages/" + galleries.ImageName));
                ImageResizer img = new ImageResizer();
                img.Resize(Server.MapPath("/Images/ProductImages/" + galleries.ImageName),
                   Server.MapPath("/Images/ProductImages/Thumb/" + galleries.ImageName));
                db.Product_GalleryiesRepository.Insert(galleries);
                db.Save();
            }
            return RedirectToAction("Gallery", new { id = galleries.ProductID });
        }
        public ActionResult DeleteGallery(int id)
        {
            var gallery = db.Product_GalleryiesRepository.GetById(id);
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/" + gallery.ImageName));
            System.IO.File.Delete(Server.MapPath("/Images/ProductImages/Thumb/" + gallery.ImageName));
            db.Product_GalleryiesRepository.Delete(gallery);
            db.Save();
            return RedirectToAction("Gallery", new { id = gallery.ProductID });
        }
        public ActionResult Features(int id)
        {
            ViewBag.Feature = db.Product_FeaturesRepository.Get(u => u.ProductID == id);
            ViewBag.FeatureID = new SelectList(db.Feature_GroupsRepository.Get(f => f.FeatureParentID != null), "FeatureGroupID", "FeatureGroupTitle");
            ViewBag.group = new SelectList(db.Feature_GroupsRepository.Get(f => f.FeatureParentID == null), "FeatureGroupID", "FeatureGroupTitle");

            var featuretest = db.Product_FeaturesRepository.GetById(id);

            return View(new Product_Features()
            {
                ProductID = id
            });
        }
        public JsonResult GetState(int CountryId)
        {

            db.config();
            var it1em = db.Feature_GroupsRepository.Get(i => i.FeatureParentID == CountryId);
            return Json(it1em, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Features(Product_Features features)
        {

            if (ModelState.IsValid && features.FeatureID != 0)
            {

                db.Product_FeaturesRepository.Insert(features);
                ViewBag.parentID = features.FeatureID;
                db.Save();

            }

            ViewBag.parentID = features.FeatureID;
            return RedirectToAction("Features", new { id = features.ProductID });



        }

        public void DeleteFea(int id)
        {
            
            var featuress= db.Product_FeaturesRepository.GetById(id);
            db.Product_FeaturesRepository.Delete(featuress);
            db.Save();


        }

    }
}
