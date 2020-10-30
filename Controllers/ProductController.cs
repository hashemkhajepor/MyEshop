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
    }
}