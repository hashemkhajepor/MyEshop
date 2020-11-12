using DataLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEshop.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        UnitOfWork db = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Slider()
        {
          

           
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            return PartialView(db.SliderRepository.Get(s => s.IsActive && s.StartDate <= dt && s.EndDate >= dt));
        }
    }
}