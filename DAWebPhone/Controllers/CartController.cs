using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class CartController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Cart
        public ActionResult CartPage() //Trang giỏ hàng chi tiết
        {
            if (SessionConfig.GetUser() != null)
            {
                decimal x = 0;
                foreach (var i in GetList())
                    x += i.TotalPrice;
                ViewBag.FinalPrice = $"{x:#,0}";
                return View(GetList());
            }
            return RedirectToAction("Login", "User");
        }
        public ActionResult PartialCart() // Giỏ trên header
        {
            if (SessionConfig.GetUser() != null)
            {
                int n = GetNumber();
                ViewBag.Count = n;
                return PartialView(GetList());
            }
            return PartialView("NotLogin", "Cart");
        }
        public ActionResult AddToCart(int id, int? page) //thêm vào giỏ
        {
            var user = SessionConfig.GetUser();
            if (user == null)
                return RedirectToAction("Login", "User");
            var item = db.Carts.Where(a => a.ProID == id && a.CusID == user.CusID).FirstOrDefault();
            if (item == null)
            {
                item = new Cart();
                item.ProID = id;
                item.CusID = user.CusID;
                item.Quantity = 1;
                db.Carts.Add(item);
                db.SaveChanges();
            }
            return RedirectToAction("Products", "Product", new { page = page });
        }
        public int GetNumber() //lấy tổng số lượng sản phẩm đã thêm vào giỏ
        {
            int x = 0;
            foreach (var i in GetList())
            {
                x += (int)i.Cart.Quantity;
            }
            return x;
        }
        public List<CartDetailViewModel> GetList() //Lấy danh sách cart
        {
            int id = SessionConfig.GetUser().CusID;
            var vms = new List<CartDetailViewModel>();
            var carts = db.Carts.Where(a => a.CusID == id);
            foreach (var cart in carts)
            {
                var vm = new CartDetailViewModel();
                vm.Cart = cart;
                vms.Add(vm);
            }
            return vms;
        }
        public decimal GetPrice()
        {
            decimal x = 0;
            foreach (var cart in GetList())
            {
                x += cart.TotalPrice;
            }
            return x;
        }
        public ActionResult NotLogin()
        {
            return PartialView();
        }
        public ActionResult Order(FormCollection form)
        {
            if (form["DeliAddress"] == "")
                return PartialView("NotInput", "ViewCart");
            //return Content("Chưa nhập thông tin thanh toán");
            int id = SessionConfig.GetUser().CusID;
            var user = db.Customers.FirstOrDefault(a => a.CusID == id);
            try
            {
                int x = int.Parse(form["PayMethod"]);
                decimal gia = GetPrice();
                if (x == 1 && user.Credit < gia)
                    return RedirectToAction("NotEnoughMoney", "ViewCart");

                foreach (var cart in GetList())
                {
                    int c = db.Devices.Count(a => a.ProID == cart.Product.ProID && a.DvStatus == 0 && a.Color == cart.Cart.Color);
                    if (c < cart.Cart.Quantity)
                        return RedirectToAction("Fail");
                }
                var odp = new OrderPro();
                odp.CusID = user.CusID;
                odp.OrdDate = DateTime.Now;
                odp.DeliAddress = form["DeliAddress"];
                if (x == 0)
                    odp.PayMethod = 0;
                else
                    odp.PayMethod = 1;
                db.OrderProes.Add(odp);
                db.SaveChanges();
                var order = db.OrderProes.FirstOrDefault(a => a.CusID == id && a.OrdStatus == 0);
                foreach (var cart in GetList())
                {
                    var ca = db.Carts.FirstOrDefault(a => a.CartID == cart.Cart.CartID);
                    for (int i = 0; i < ca.Quantity; i++)
                    {
                        var device = db.Devices.FirstOrDefault(a => a.ProID == cart.Product.ProID && a.DvStatus == 0 && a.Color == cart.Cart.Color);
                        var odt = new OrderDetail();
                        odt.OrdID = order.OrdID;
                        odt.Code = device.Code;
                        device.DvStatus = 1;
                        db.OrderDetails.Add(odt);
                        db.SaveChanges();
                    }
                    db.Carts.Remove(ca);
                }
                if (x == 1)
                {
                    user.Credit -= gia;
                }
                order.OrdStatus = 1;
                order.TotalPrice = gia;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.SaveChanges();
                SessionConfig.SetUser(user);
                return RedirectToAction("OrderSuccess");
            }
            catch
            {
                return RedirectToAction("Fail");
            }
        }

        public ActionResult NotInput()
        {
            return PartialView();
        }
        public ActionResult OrderSuccess()
        {
            return PartialView();
        }
        public ActionResult Delete(int id, int? page)
        {
            var cart = db.Carts.FirstOrDefault(a => a.CartID == id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            if (page == null)
                return RedirectToAction("Index", "Home");
            return RedirectToAction("CartPage", "Cart");
        }
        public ActionResult Update(FormCollection f)
        {
            if (f != null)
            {
                int id = int.Parse(f["id"]);
                var cart = db.Carts.FirstOrDefault(a => a.CartID == id);
                if (f["Quantity"] != "")
                {
                    int num = int.Parse(f["Quantity"]);
                    cart.Quantity = num > 0 ? num : 1;
                }
                cart.Color = f["Color"];
                db.SaveChanges();
            }
            return RedirectToAction("CartPage", "Cart");
        }
        public ActionResult Fail()
        {
            ViewBag.Fail = "Sản phẩm tạm hết hàng, chúng rất xin lỗi vì điều này";
            return View();
        }
        public ActionResult NotEnoughMoney()
        {
            ViewBag.Fail = "Số dư không đủ đặt hàng!";
            return View();
        }
        public ActionResult Increase(int id)
        {
            var cart = db.Carts.FirstOrDefault(a => a.CartID == id);
            if (cart != null)
            {
                cart.Quantity++;
                db.SaveChanges();
            }
            return RedirectToAction("CartPage");
        }
    }
}