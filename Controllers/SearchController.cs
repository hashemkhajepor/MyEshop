using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace MyEshop.Controllers
{
    public class SearchController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Search
        public ActionResult Index(string q)
        {
            List<Products> list = new List<Products>();
            list.AddRange(db.Product_TagsRepository.Get(t => t.Tag == q).Select(t => t.Products).ToList());
            list.AddRange(db.ProductsRepository.Get(p => p.Title.Contains(q) || p.ShortDescription.Contains(q) || p.Text.Contains(q)).ToList());
            ViewBag.search = q;
            return View(list.Distinct());
        }
    }
}