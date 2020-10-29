using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MyEshop.Areas.UserPanel.Controllers
{
    public class AccountController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: UserPanel/Account
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangPasswordViewModel chang)
        {
            if (ModelState.IsValid)
            {
                var user = db.UsersRepository.Get().Single(u => u.UserName == User.Identity.Name);
                string oldPasswordhash = FormsAuthentication.HashPasswordForStoringInConfigFile(chang.OldPassword, "MD5");
                if(user.Password == oldPasswordhash)
                {
                    string hashnewpassword = FormsAuthentication.HashPasswordForStoringInConfigFile(chang.Password, "MD5");
                    user.Password = hashnewpassword;
                    db.Save();
                    ViewBag.Success = true;
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "کلمه عبور فعلی درست نمی باشد");
                }
            }
            return View();
        }
    }
}