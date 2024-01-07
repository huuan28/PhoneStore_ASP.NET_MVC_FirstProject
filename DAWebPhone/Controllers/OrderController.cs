using DAWebPhone.App_Start;
using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

namespace DAWebPhone.Controllers
{
    public class OrderController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Order
        public ActionResult ViewUserOrder(int id)
        {
            var ord = db.OrderProes.FirstOrDefault(a => a.OrdID == id);
            if (ord != null)
            {
                var od = new OrderViewModel();
                od.OrderPro = ord;
                return View(od);
            }
            return RedirectToAction("OrderHistory");
        }
        public ActionResult BuyNow(int ProID, int? page)
        {
            var cus = SessionConfig.GetUser();
            if (cus == null)
                return RedirectToAction("Login", "User");
            var pro = db.Products.FirstOrDefault(a => a.ProID == ProID);
            ViewBag.Cus = cus.CusID;
            var vm = new ProductDetailViewModel();
            vm.Product = pro;
            return View(vm);
        }
        public ActionResult OrderNow(FormCollection f)
        {
            if (f != null)
            {
                if (string.IsNullOrEmpty(f["DeliAddress"]))//khong dien dia chi
                    return RedirectToAction("NotInput", "Cart");
                int sl = int.Parse(f["Quantity"]);
                if (sl < 1)//so luong nho hon 1
                    return RedirectToAction("Fail", "Cart");
                int ProID = int.Parse(f["ProID"].ToString());
                var pro = db.Products.FirstOrDefault(a => a.ProID == ProID);

                int count = db.Devices.Count(a => a.ProID == ProID && a.DvStatus == 0);
                if (count < sl) //so luong ton khong du
                    return RedirectToAction("Fail", "Cart");
                string color;
                if (f["Color"] == null)
                    color = "";
                else
                    color = f["Color"].ToString();
                var cus = SessionConfig.GetUser(); //lay thong tin nguoi dung
                var user = db.Customers.FirstOrDefault(a => a.CusID == cus.CusID); //tim doi tuong o databasecontext
                var ord = new OrderPro(); //tao moi don hang
                ord.CusID = cus.CusID;
                db.OrderProes.Add(ord); //them vao database de cap nhat id don hang
                var odp = db.OrderProes.FirstOrDefault(a => a.CusID == ord.CusID && a.OrdStatus == 0); //lay don hang da co id
                odp.OrdDate = DateTime.Now;
                odp.TotalPrice = pro.FinalPrice * sl;
                int pm = int.Parse(f["PayMethod"]);
                odp.PayMethod = pm;
                if (pm == 1 && user.Credit < ord.TotalPrice)//tai khoan khong du tien neu chon hinh thuc tru tai khoan
                    return RedirectToAction("NotEnoughMoney", "Cart");
                if (pm == 1) //cap nhat lai so du tai khoan
                {
                    user.Credit -= ord.TotalPrice;
                    db.SaveChanges();
                    SessionConfig.SetUser(user); //cap nhat session nguoi dung
                }
                ord.CusID = int.Parse(f["CusID"]);
                for (int i = 0; i < sl; i++)
                {
                    var odt = new OrderDetail();//them chi tiet don
                    var dv = db.Devices.FirstOrDefault(a => a.ProID == ProID && a.DvStatus == 0);
                    dv.DvStatus = 1;//cap nhat trang thai thiet bi
                    odt.Code = dv.Code;
                    odt.OrdID = odp.OrdID;
                    db.OrderDetails.Add(odt);
                    db.SaveChanges();
                }
                odp.OrdStatus = 1;//chuyen don hang sang trang thai cho xu ly
                db.SaveChanges();
            }
            return RedirectToAction("OrderSuccess", "Cart");
        }
        public ActionResult OrderHistory(int id)
        {
            var list = db.OrderProes.Where(a => a.CusID == id).ToList();
            List<OrderViewModel> vms = new List<OrderViewModel>();
            foreach (var item in list)
            {
                var vm = new OrderViewModel();
                vm.OrderPro = item;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult CancelOrder(int id)
        {
            var ord = db.OrderProes.FirstOrDefault(a => a.OrdID == id);
            if (ord != null)
            {
                return View(ord);
            }
            return RedirectToAction("OrderHistory", new { id = ord.CusID });
        }
        [HttpPost]
        public ActionResult CancelOrder(OrderPro ord)
        {
            if (ord != null)
            {
                var odp = db.OrderProes.FirstOrDefault(a => a.OrdID == ord.OrdID);
                odp.OrdStatus = 3;
                if (odp.PayMethod == 1)
                {
                    var user = db.Customers.FirstOrDefault(a => a.CusID == ord.CusID);
                    if (user != null)
                    {
                        user.Credit += odp.TotalPrice;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        SessionConfig.SetUser(user);
                    }
                }
                var odList = db.OrderDetails.Where(a => a.OrdID == odp.OrdID).ToList();
                foreach (var i in odList)
                {
                    var dv = db.Devices.FirstOrDefault(a => a.Code == i.Code);
                    dv.DvStatus = 0;
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
            return RedirectToAction("OrderHistory", new { id = ord.CusID });
        }
        public ActionResult Wait()
        {
            var adm = SessionConfig.GetAdm();
            if (adm == null)
                return RedirectToAction("Login", "Admin");
            var list = db.OrderProes.Where(a => a.OrdStatus == 1).ToList();
            List<OrderViewModel> vms = new List<OrderViewModel>();
            foreach (var item in list)
            {
                var vm = new OrderViewModel();
                vm.OrderPro = item;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult Refuse(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var ord = db.OrderProes.FirstOrDefault(a => a.OrdID == id);
            if (ord != null)
            {
                if (ord.PayMethod == 1)
                {
                    var user = db.Customers.FirstOrDefault(a => a.CusID == ord.CusID);
                    if (user != null)
                    {
                        user.Credit += ord.TotalPrice;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        SessionConfig.SetUser(user);
                    }
                }
                var odList = db.OrderDetails.Where(a => a.OrdID == ord.OrdID).ToList();
                foreach (var i in odList)
                {
                    var dv = db.Devices.FirstOrDefault(a => a.Code == i.Code);
                    dv.DvStatus = 0;
                    db.SaveChanges();
                }
                ord.Handler = SessionConfig.GetAdm().AdmID;
                ord.OrdStatus = 4;
                db.SaveChanges();
            }
            return RedirectToAction("Wait");
        }
        public ActionResult Accept(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var ord = db.OrderProes.FirstOrDefault(a => a.OrdID == id);
            if (ord != null)
            {
                ord.Handler = SessionConfig.GetAdm().AdmID;
                ord.AcceptedDate = DateTime.Now;
                ord.OrdStatus = 2;
                db.SaveChanges();
                var odList = db.OrderDetails.Where(a => a.OrdID == ord.OrdID).ToList();
                foreach (var i in odList)
                {
                    var dv = db.Devices.FirstOrDefault(a => a.Code == i.Code);
                    dv.DvStatus = 2;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Wait");
        }
        public ActionResult ViewAdminOrder(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login");
            var ord = db.OrderProes.FirstOrDefault(a => a.OrdID == id);
            if (ord != null)
            {
                var od = new OrderViewModel();
                od.OrderPro = ord;
                return View(od);
            }
            return RedirectToAction("ViewAllOrders");
        }
        public ActionResult ViewAllOrders(int? id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            int x = id ?? 0;

            List<OrderPro> list;
            switch (x)
            {
                case 2:
                    list = db.OrderProes.Where(a => a.OrdStatus == 2).ToList();
                    break;
                case 3:
                    list = db.OrderProes.Where(a => a.OrdStatus == 3).ToList();
                    break;
                case 4:
                    list = db.OrderProes.Where(a => a.OrdStatus == 4).ToList();
                    break;
                default:
                    list = db.OrderProes.ToList();
                    break;
            }
            var vms = new List<OrderViewModel>();
            foreach (var order in list)
            {
                var vm = new OrderViewModel();
                vm.OrderPro = order;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult TopBill()
        {
            var query = from c in db.OrderProes
                        where c.OrdStatus == 2
                        orderby c.TotalPrice descending
                        select c;
            var list = query.Take(10).ToList();
            return View(list);
        }
    }
}