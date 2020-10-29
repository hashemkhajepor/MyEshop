using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;

namespace MyEshop.Controllers
{
    public class AccountController : Controller
    {
        UnitOfWork db = new UnitOfWork();
        // GET: Account
        [Route("Register")]
        public ActionResult Register()
        {
            return View();
        }
        [Route("Register")]
        [HttpPost]
        public ActionResult Register(RegisterViewModel register)
        {
            if(!db.UsersRepository.Get(u=>u.Email == register.Email.Trim().ToLower()).Any())
            {
                Users user = new Users()
                {
                    UserName = register.UserName,
                    Email = register.Email.Trim().ToLower(),
                    Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Password, "MD5"),
                    ActiveCode = Guid.NewGuid().ToString(),
                    IsActive = false,
                    RegisterDate = DateTime.Now,
                    RoleID = 1
                };
                string body = PartialToStringClass.RenderPartialView("ManageEmails", "ActivationEmail", user);
                SendEmail.Send(register.Email, "فعال سازی حساب در فروشگاه", body);
                db.UsersRepository.Insert(user);
                db.Save();
                return View("SuccessRegister", user);
            }
            else
            {
                ModelState.AddModelError("Email", "این ایمیل قبلا استفاده شده است");
            }
            return View();
        }
        public ActionResult ActiveUser(string id)
        {
            var user = db.UsersRepository.Get(u => u.ActiveCode == id).SingleOrDefault();
            if(user == null)
            {
                return HttpNotFound();
            }
            user.ActiveCode = Guid.NewGuid().ToString();
            user.IsActive = true;
            db.Save();
            ViewBag.UserName = user.UserName;
            return View();
        }
        [Route("Login")]
        public ActionResult Login()
        {

            return View();
        }
        [Route("Login")]
        [HttpPost]
        public ActionResult Login(LoginViewModel login , string ReturnUrl="/")
        {
            if (ModelState.IsValid)
            {

                var hashPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(login.Password, "MD5");
                var user = db.UsersRepository.Get(u => u.Email == login.Email && u.Password == hashPassword).SingleOrDefault();
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, login.RemmeberMi);
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "حساب کاربری شما فعال نشده است");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "کاربری یافت نشد.");
                }
            }
            return View();
        }

        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [Route("ForgotPassword")]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgot)
        {
            if (ModelState.IsValid)
            {
                var user = db.UsersRepository.Get(u => u.Email == forgot.Email).SingleOrDefault();
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        string Body = PartialToStringClass.RenderPartialView("ManageEmails", "RecoveryEmail", user);
                        SendEmail.Send(forgot.Email, "بازیابی کلمه عبور", Body);
                        return View("SuccessForgotPassword", user);

                    }
                    else
                    {
                        ModelState.AddModelError("Email", "حساب کاربری شما فعال نیست");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "کاربری با ایمیل وارد شده یافت نشد");
                }
            }
            return View();
        }

        public ActionResult RecoveryPassword(string id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecoveryPassword(string id, RecoveryPasswordViewModel recovery)
        {
            if (ModelState.IsValid)
            {
                var user = db.UsersRepository.Get().SingleOrDefault(u=>u.ActiveCode == id);
                if(user == null)
                {
                  return  HttpNotFound();
                }
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(recovery.Password, "MD5");
                user.ActiveCode = Guid.NewGuid().ToString();
                db.Save();
                return Redirect("/Login?recovery=true");
            }
            return View();
        }
    }
}