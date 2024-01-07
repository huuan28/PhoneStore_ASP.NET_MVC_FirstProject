using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class AdminController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Admin

        public ActionResult DashBoard()
        {
            var adm = SessionConfig.GetAdm();
            if (adm == null)
                return RedirectToAction("Login");
            var d = db.OrderProes.Where(b => b.OrdStatus == 2 && b.AcceptedDate.Value.Day == DateTime.Now.Day).ToList();
            var m = db.OrderProes.Where(b => b.OrdStatus == 2 && b.AcceptedDate.Value.Month == DateTime.Now.Month).ToList();
            var y = db.OrderProes.Where(b => b.OrdStatus == 2 && b.AcceptedDate.Value.Year == DateTime.Now.Year).ToList();
            var all = db.OrderProes.Where(b => b.OrdStatus == 2).ToList();
            int cD = d.Count;
            int cM = m.Count;
            int cY = y.Count;
            decimal mD = 0, mM = 0, mY = 0, mA = 0;
            foreach (var i in d)
            {
                mD += i.TotalPrice;
            }
            foreach (var i in m)
            {
                mM += i.TotalPrice;
            }
            foreach (var i in y)
            {
                mY += i.TotalPrice;
            }
            foreach (var i in all)
            {
                mA += i.TotalPrice;
            }
            int cAll = all.Count;
            ViewBag.CountD = cD;
            ViewBag.CountM = cM;
            ViewBag.CountY = cY;
            ViewBag.TotalCount = cAll;
            ViewBag.MD = $"{mD:#,0} đ";
            ViewBag.MM = $"{mM:#,0} đ";
            ViewBag.MY = $"{mY:#,0} đ";
            ViewBag.MA = $"{mA:#,0} đ";
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminUser adm)
        {
            var check = db.AdminUsers.FirstOrDefault(c => c.AdmLogName == adm.AdmLogName && c.AdmPass == adm.AdmPass);
            if (check != null)
            {
                SessionConfig.SetAdm(check);
                return RedirectToAction("DashBoard");
            }
            ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }
        public ActionResult Logout()
        {
            SessionConfig.SetAdm(null);
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AdminUser adm)
        {
            if (ModelState.IsValid)
            {
                var check = db.AdminUsers.FirstOrDefault(c => c.AdmName == adm.AdmName);
                if (check != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                    return View();
                }
                else
                {
                    db.AdminUsers.Add(adm);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }
        //=============Register=================//

        public ActionResult EditProfile(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var adm = db.AdminUsers.FirstOrDefault(a => a.AdmID == id);
            if (adm != null)
            {
                return View(adm);
            }
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(AdminUser adm)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                var ad = db.AdminUsers.FirstOrDefault(a => a.AdmID == adm.AdmID);
                if (ad != null)
                {
                    if (adm.UploadImage != null)
                    {
                        //Lấy tên file của hình được up lên
                        var fileName = Path.GetFileName(adm.UploadImage.FileName);
                        //Tạo đường dẫn tới file
                        var path = Path.Combine(Server.MapPath("~/Images/AdmAvt/"), fileName);
                        //Lưu tên
                        ad.AdmAvatar = "/Images/AdmAvt/" + fileName;
                        //Save vào Images Folder
                        adm.UploadImage.SaveAs(path);
                    }

                    ad.AdmName = adm.AdmName;
                    ad.AdmPass = adm.AdmPass;
                    ad.ConfirmPass = adm.ConfirmPass;
                    ad.AdmEmail = adm.AdmEmail;
                    ad.AdmActive = adm.AdmActive;
                    ad.AdmGender = adm.AdmGender;
                    ad.AdmAddress = adm.AdmAddress;
                    ad.AdmBirth = adm.AdmBirth;
                    ad.AdmCreDate = adm.AdmCreDate;
                    db.SaveChanges();
                    return RedirectToAction("Profile", "Admin", new { id = adm.AdmID });
                }
            }
            ViewBag.Error = "Mật khẩu không đúng!";
            return View();
        }
        public ActionResult Profile(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var adm = db.AdminUsers.FirstOrDefault(a => a.AdmID == id);
            if (adm != null)
                return View(adm);
            return RedirectToAction("Dashboard");
        }
        public ActionResult PartialProfile()
        {
            var adm = SessionConfig.GetAdm();
            return PartialView(adm);
        }
    }
}