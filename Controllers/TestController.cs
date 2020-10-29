using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEshop.Controllers
{
    
    public class TestController : Controller
    {
        [Authorize]
        // GET: Test
        public ActionResult test1()
        {
            return View();
        }
        [Authorize(Roles ="admin")]
        public ActionResult test2()
        {
            return View();
        }
    }
}