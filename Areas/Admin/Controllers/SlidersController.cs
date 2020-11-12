using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace MyEshop.Areas.Admin.Controllers
{
    public class SlidersController : Controller
    {
        UnitOfWork db = new UnitOfWork();

        // GET: Admin/Sliders
        public ActionResult Index()
        {
            return View(db.SliderRepository.Get());
        }

        // GET: Admin/Sliders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.SliderRepository.GetById(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sliders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SlideID,Title,Url,ImageName,StartDate,EndDate,IsActive")] Slider slider , HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid)
            {
                if(imgUp == null)
                {
                    ModelState.AddModelError("ImageName", "لطفا تصویر را انتخاب کنید");
                    return View(slider);
                }
                PersianCalendar pc = new PersianCalendar();
                slider.StartDate = new DateTime(slider.StartDate.Year, slider.StartDate.Month, slider.StartDate.Day, pc);
                slider.EndDate = new DateTime(slider.EndDate.Year, slider.EndDate.Month, slider.EndDate.Day, pc);
                slider.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imgUp.FileName);
                imgUp.SaveAs(Server.MapPath("/Images/Slider/" + slider.ImageName));

                db.SliderRepository.Insert(slider);
                db.Save();
                return RedirectToAction("Index");

          
            }

            return View(slider);
        }

        // GET: Admin/Sliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.SliderRepository.GetById(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SlideID,Title,Url,ImageName,StartDate,EndDate,IsActive")] Slider slider , HttpPostedFileBase imgUp)
        {
            if (ModelState.IsValid)
            {
                if (imgUp != null)
                {
                    System.IO.File.Delete(Server.MapPath("/Images/Slider/" + slider.ImageName));
                
                    slider.ImageName = Guid.NewGuid().ToString() + Path.GetExtension(imgUp.FileName);
                    imgUp.SaveAs(Server.MapPath("/Images/Slider/" + slider.ImageName));
                   
                }
                PersianCalendar pc = new PersianCalendar();
                slider.StartDate = new DateTime(slider.StartDate.Year, slider.StartDate.Month, slider.StartDate.Day, pc);
                slider.EndDate = new DateTime(slider.EndDate.Year, slider.EndDate.Month, slider.EndDate.Day, pc);
                db.SliderRepository.Update(slider);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(slider);
        }

        // GET: Admin/Sliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.SliderRepository.GetById(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Admin/Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slider slider = db.SliderRepository.GetById(id);
            System.IO.File.Delete(Server.MapPath("/Images/Slider/" + slider.ImageName));
            db.SliderRepository.Delete(id);
            db.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
