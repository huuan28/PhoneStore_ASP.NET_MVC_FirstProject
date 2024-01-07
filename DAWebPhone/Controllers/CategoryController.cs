using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class CategoryController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Category
        public ActionResult CateList()
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var list = db.Categories.ToList();
            return View(list);
        }
        public ActionResult Create()
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category cat)
        {
            if(ModelState.IsValid)
            {
                var check = db.Categories.FirstOrDefault(m=>m.CatName == cat.CatName);
                if(check != null)
                {
                    ModelState.AddModelError("", "Tên đã tồn tại!");
                    return View();
                }
                else
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    return RedirectToAction("CateList");
                }
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var ca = db.Categories.FirstOrDefault(a => a.CatID == id);
            return View(ca);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category ca)
        {
            if (ModelState.IsValid)
            {
                var cat = db.Categories.FirstOrDefault(a => a.CatName == ca.CatName);
                if (cat != null)
                {
                    ModelState.AddModelError("", "Tên Hãng đã tồn tại");
                    return View();
                }
                cat.CatName = ca.CatName;
                db.SaveChanges();
            }
            return RedirectToAction("BraList");
        }
        public ActionResult Delete(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var ca = db.Categories.FirstOrDefault(a => a.CatID == id);
            return View(ca);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Category ca)
        {
            var cat = db.Categories.FirstOrDefault(a => a.CatID == ca.CatID);
            db.Categories.Remove(cat);
            db.SaveChanges();
            return RedirectToAction("BraList");
        }
        public ActionResult SelectCate()
        {
            var se = new Category();
            se.CateList = db.Categories.ToList();
            return PartialView(se);
        }
        public ActionResult PartialList()
        {
            var list = db.Categories.ToList();
            return PartialView(list);
        }
        public ActionResult Partialmenu()
        {
            var list = db.Categories.ToList();
            return PartialView(list);
        }
    }
}