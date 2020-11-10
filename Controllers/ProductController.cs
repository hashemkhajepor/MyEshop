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
        public ActionResult ShowComments(int id)
        {
            return PartialView(db.ProductCommentsRepository.Get(p => p.ProductID == id));
        }

        public ActionResult CreateComment(int id)
        {
            return PartialView(new Product_Comments()
            {
                ProductID = id
            });
        }

        [HttpPost]
        public ActionResult CreateComment(Product_Comments productComment)
        {
            if (ModelState.IsValid)
            {
                productComment.CreateDate = DateTime.Now;
                db.ProductCommentsRepository.Insert(productComment);
                db.Save();
                return PartialView("ShowComments", db.ProductCommentsRepository.Get(p => p.ProductID == productComment.ProductID));
            }
            return PartialView(productComment);
        }

        [Route("Archive")]
        public ActionResult ArchiveProduct(int pageId = 1, string Title = "", int minPrice = 0, int maxPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.Groups = db.Product_GroupsRepository.Get();
            ViewBag.productTitle = Title;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            ViewBag.pageId = pageId;
            ViewBag.selectedGroup = selectedGroups;
            List<Products> list = new List<Products>();
            if (selectedGroups != null && selectedGroups.Any())
            {
                foreach (int group in selectedGroups)
                {
                    list.AddRange(db.Product_Selected_GroupsRepository.Get(g => g.GroupID == group).Select(g => g.Products).ToList());
                }
                list = list.Distinct().ToList();
            }
            else
            {
                list.AddRange(db.ProductsRepository.Get());
            }
            if (Title != " ")
            {
                list = list.Where(p => p.Title.Contains(Title)).ToList();
            }
            if (minPrice > 0)
            {
                list = list.Where(p => p.Price >= minPrice).ToList();
            }
            if (maxPrice > 0)
            {
                list = list.Where(p => p.Price <= maxPrice).ToList();
            }

            int take = 4;
            int Skip = (pageId - 1) * take;
            ViewBag.PageCount = list.Count() / take;
            if ((list.Count() % take) != 0)
            {
                ViewBag.PageCount += 1;
            }
            return View(list.OrderByDescending(p => p.CreateDate).Skip(Skip).Take(take).ToList());
        }
    }
}