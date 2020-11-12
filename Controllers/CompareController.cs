using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace MyEshop.Controllers
{
    public class CompareController : Controller
    {
        // GET: Compare
        UnitOfWork db = new UnitOfWork();
        public ActionResult Index()
        {
            List<CompareItem> list = new List<CompareItem>();
            List<Feature_Groups> features = new List<Feature_Groups>();
            List<Product_Features> product_Features = new List<Product_Features>();
            if(Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;
            }
            foreach(var item in list)
            {
                features.AddRange(db.Product_FeaturesRepository.Get(p => p.ProductID == item.ProductID).Select(p => p.Feature_Groups).ToList());
                product_Features.AddRange(db.Product_FeaturesRepository.Get(p => p.ProductID == item.ProductID).ToList());
            }
            ViewBag.features = features.Distinct().ToList();
            ViewBag.productfeatures = product_Features;
            return View(list);
        }
        public ActionResult AddToCompare(int id)
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;

            }
            var product = db.ProductsRepository.Get(u => u.ProductID == id).Select(u => new
            {
                u.Title,
                u.ImageName
            }).Single();
            if (!list.Any(p => p.ProductID == id))
            {
                list.Add(new CompareItem()
                {
                    ProductID = id,
                    ImageName = product.ImageName,
                    Title = product.Title
                });
            }
            Session["Compare"] = list;
            return PartialView("ListCompare" , list);
        }
        public ActionResult ListCompare()
        {
            List<CompareItem> list = new List<CompareItem>();
            if(Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;
            }
            return PartialView(list);
        }
        
        public ActionResult DeleteFromCompare(int id)
        {
            List<CompareItem> list = new List<CompareItem>();
            if (Session["Compare"] != null)
            {
                list = Session["Compare"] as List<CompareItem>;
                int index = list.FindIndex(p => p.ProductID == id);
                list.RemoveAt(index);
                Session["Compare"] = list;

            }

            return PartialView("ListCompare", list);
        }

        
        
    }
}