using DataLayer;
using DataLayer.ViewModelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MyEshop.Controllers
{
    public class ShopCartController : Controller
    {
        // GET: ShopCart
        UnitOfWork db = new UnitOfWork();
        public ActionResult ShowCart()
        {

            return PartialView(getlistOrder());
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommandOrder(int id, int count)
        {
            List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;
            int index = listShop.FindIndex(p => p.ProductID == id);
            if (count == 0)
            {
                listShop.RemoveAt(index);
            }
            else
            {
                listShop[index].Count = count;
            }
            Session["ShopCart"] = listShop;
            return PartialView("Order", getlistOrder());
        }
        public ActionResult Order()
        {


            return PartialView(getlistOrder());
        }
        List<ShopCartItemViewModel> getlistOrder()
        {
            List<ShopCartItemViewModel> list = new List<ShopCartItemViewModel>();
            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;
                foreach (var item in listShop)
                {
                    var product = db.ProductsRepository.Get(p => p.ProductID == item.ProductID).Select(p => new
                    {
                        p.ImageName,
                        p.Title,
                        p.PriceOff,
                        p.Price
                    }).Single();
                    list.Add(new ShopCartItemViewModel()
                    {
                        Count = item.Count,
                        ProductID = item.ProductID,
                        ImageName = product.ImageName,
                        Price = product.PriceOff,
                        Title = product.Title,
                        Sum = item.Count * product.PriceOff
                    });
            }
        }
            return list;
        }

        [Authorize]
        public ActionResult Payment()
        {
            int userId = db.UsersRepository.Get(u => u.UserName == User.Identity.Name).Single().UserID;
            Orders order = new Orders()
            {
                UserID = userId,
                Date = DateTime.Now,
                IsFinaly = false

            };

            db.OrdersRepository.Insert(order);
            var listDetails = getlistOrder();
            foreach(var itme in listDetails)
            {
                db.OrderDetailsRepository.Insert(new OrderDetails()
                {
                   Count = itme.Count,
                   OrderID = order.OrderID,
                   Price= itme.Price,
                   ProductID = itme.ProductID,
                   
                });
            }
            db.Save();
            int hasel = db.OrderDetailsRepository.Get(o => o.OrderID == order.OrderID).Sum(p => p.Price);

           
            System.Net.ServicePointManager.Expect100Continue = false;
            ZarinPal.PaymentGatewayImplementationServicePortTypeClient
                zp = new ZarinPal.PaymentGatewayImplementationServicePortTypeClient();
            string Authority;
            int Status = zp.PaymentRequest("YOUR-ZARINPAL-MERCHANT-CODE", hasel, "فروشگاه آنلاین", "hashemkhajepour2014@gmail.com", "09164939935", "http://localhost:62986/ShopCart/Verify/" + order.OrderID, out Authority);

            if (Status == 100)
            {
                Response.Redirect("http://sandbox.zarinpal.com/pg/StartPay/" + Authority);
            }
            else
            {
                ViewBag.Error = "Error :" + Status;
            }
            return View();
        }

        public ActionResult Verify(int id)
        {
            var order = db.OrdersRepository.GetById(id);
            int hasel = db.OrderDetailsRepository.Get(o => o.OrderID == id).Sum(p => p.Price);
            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    int Amount = hasel;
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    ZarinPal.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinPal.PaymentGatewayImplementationServicePortTypeClient();
                    int Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), Amount, out RefID);

                    if (Status == 100)
                    {
                        order.IsFinaly = true;
                        db.Save();
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                    }
                    else
                    {
                        ViewBag.Status = Status;
                    }
                }


            }
            else
            {
                Response.Write("Invalid Input");
            }
            return View();
        }
    }
}
