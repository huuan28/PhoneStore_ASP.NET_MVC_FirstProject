using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class UserController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartialProfile()
        {
            var user = SessionConfig.GetUser();
            if (user != null)
            {
                return PartialView(user);
            }
            return PartialView("BtnLogin");
        }
        public ActionResult BtnLogin()
        {
            return PartialView();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Customer cus)
        {
            var check = db.Customers.FirstOrDefault(c => c.CusLogName == cus.CusLogName && c.CusPass == cus.CusPass);
            if (check != null)
            {
                SessionConfig.SetUser(check);
                return RedirectToAction("Index","Home");
            }
            ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer cus)
        {
            if (ModelState.IsValid)
            {
                var check = db.Customers.FirstOrDefault(c => c.CusName == cus.CusName);
                if (check != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                    return View();
                }
                else
                {
                    db.Customers.Add(cus);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public ActionResult EditProfile(int id)
        {
            if (SessionConfig.GetUser() == null)
                return RedirectToAction("Login");
            var cus = db.Customers.FirstOrDefault(a => a.CusID == id);
            if (cus != null)
            {
                return View(cus);
            }
            return RedirectToAction("Profile");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(Customer Cus)
        {
            if (SessionConfig.GetUser() == null)
                return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                var cs = db.Customers.FirstOrDefault(a => a.CusID == Cus.CusID);
                if (cs != null)
                {
                    if (Cus.UploadImage != null)
                    {
                        //Lấy tên file của hình được up lên
                        var fileName = Path.GetFileName(Cus.UploadImage.FileName);
                        //Tạo đường dẫn tới file
                        var path = Path.Combine(Server.MapPath("~/Images/AdmAvt/"), fileName);
                        //Lưu tên
                        cs.CusAvatar = "/Images/AdmAvt/" + fileName;
                        //Save vào Images Folder
                        Cus.UploadImage.SaveAs(path);
                    }
                    cs.CusName = Cus.CusName;
                    cs.CusPass = Cus.CusPass;
                    cs.ConfirmPass = Cus.ConfirmPass;
                    cs.CusEmail = Cus.CusEmail;
                    cs.CusGender = Cus.CusGender;
                    cs.CusAddress = Cus.CusAddress;
                    cs.CusBirth = Cus.CusBirth;
                    cs.CusPhone = Cus.CusPhone;
                    db.SaveChanges();
                    return RedirectToAction("Profile", "User", new { id = Cus.CusID });
                }
            }
            ViewBag.Error = "Mật khẩu không đúng!";
            return View(Cus);
        }
        public ActionResult Profile(int id)
        {
            if (SessionConfig.GetUser() == null)
                return RedirectToAction("Login");
            var Cus = db.Customers.FirstOrDefault(a => a.CusID == id);
            if (Cus != null)
                return View(Cus);
            return RedirectToAction("Products","ViewProduct");
        }
        public ActionResult Logout()
        {
            SessionConfig.SetUser(null);
            return RedirectToAction("Login", "User");
        }
        public ActionResult UserList(int? id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login","Admin");
            int x = id ?? 0;
            switch (x)
            {
                //tìm top 10 VIP
                case 1:
                    var l = from a in db.Customers
                            where a.VIP > 1
                            orderby a.VIP descending
                            select a;
                    ViewBag.p = 1;
                    return View(l.Take(10));

                //danh sách khách hàng chưa mua hàng
                case 2:
                    var query = from c in db.Customers
                                where !(from o in db.OrderProes
                                        where o.OrdStatus == 2
                                        select o.CusID).Contains(c.CusID)
                                select c;
                    ViewBag.p = 2;

                    return View(query);
                //top 10 khách hay hủy đơn
                case 3:
                    query = from c in db.Customers
                            where (from o in db.OrderProes
                                   where o.OrdStatus == 3
                                   select o.CusID).Contains(c.CusID)
                            select c;
                    return View(query.Take(10));
                default:
                    return View(db.Customers.ToList());
            }
        }
    }
}