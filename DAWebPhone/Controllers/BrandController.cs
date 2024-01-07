using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class BrandController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Brand
        public ActionResult BraList()
        {
            var list = db.Brands.ToList();
            return View(list);
        }
        public ActionResult PartialList()
        {
            var list = db.Brands.ToList();
            return PartialView(list);
        }
        public ActionResult Create()
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var bra = new Brand();
            return View(bra);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand)
        {
            if(ModelState.IsValid)
            {
                db.Brands.Add(brand);
                db.SaveChanges();
            }
            return RedirectToAction("BraList");
        }
        public ActionResult SelectBrand()
        {
            var se = new Brand();
            se.BrandList = db.Brands.ToList();
            return PartialView(se);
        }
        public ActionResult Edit(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var br = db.Brands.FirstOrDefault(a=>a.BraID == id);
            return View(br);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brand br)
        {
            if(ModelState.IsValid)
            {
                var bra = db.Brands.FirstOrDefault(a=>a.BraName == br.BraName);
                if(bra != null)
                {
                    ModelState.AddModelError("","Tên Hãng đã tồn tại");
                return View();
                }
                bra.BraName = br.BraName;
                db.SaveChanges();
            }
            return RedirectToAction("BraList");
        }
        public ActionResult Delete(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var bra = db.Brands.FirstOrDefault(a=>a.BraID == id);
            return View(bra);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Brand br)
        {
            var bra = db.Brands.FirstOrDefault(a=>a.BraID==br.BraID);
            db.Brands.Remove(bra);
            db.SaveChanges();
            return RedirectToAction("BraList");
        }
    }
}