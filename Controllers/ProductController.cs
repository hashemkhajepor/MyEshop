using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEshop.Controllers
{
    public class ProductController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Product
        public ActionResult ShowGroups()
        {
            return PartialView(db.Product_GroupsRepository.Get());
        }

        public ActionResult AmazonOffer()
        {
            ViewBag.ProductPrice = db.ProductsRepository.Get(i => i.PriceOff != 0);
            return PartialView();
        }

        public ActionResult LastProduct()
        {
            return PartialView(db.ProductsRepository.Get().OrderByDescending(p => p.CreateDate).Take(12));
        }

        public ActionResult ShowProduct(int id)
        {
            var product = db.ProductsRepository.GetById(id);
            if(product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Amazing()
        {
            ViewBag.ProductPrices = db.ProductsRepository.Get(i => i.PriceOff != 0).ToList();
            return PartialView();
        }
        public ActionResult ProductFeatures(int id)
        {
            var product = db.ProductsRepository.GetById(id);

            ViewBag.featuresub = product.Product_Features.Select(o => o.Feature_Groups.FeatureParentID.Value).Distinct().ToList();



            ViewBag.ProductFeature = db.Product_FeaturesRepository.Get(a => a.ProductID == id);





            return PartialView(db.Feature_GroupsRepository.Get());
        }

       

    }
}