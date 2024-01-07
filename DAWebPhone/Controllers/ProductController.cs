using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using DAWebPhone.App_Start;

namespace DAWebPhone.Controllers
{
    public class ProductController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Product
        public ActionResult Add()
        {
            //
            //var user = db.AdminUsers.FirstOrDefault();
            //SessionConfig.SetAdm(user);
            //
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var pro = new Product();
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product pro)
        {
            if (ModelState.IsValid)
            {
                var check = db.Products.FirstOrDefault(m => m.ProName == pro.ProName);
                if (check != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên sản phẩm đã tồn tại");
                    return View(pro);
                }
                else
                {
                    if (pro.UploadImage != null)
                    {
                        //Lấy tên file của hình được up lên
                        var fileName = Path.GetFileName(pro.UploadImage.FileName);
                        string pre = pro.ProName + "/";
                        var path = Path.Combine(Server.MapPath("~/Images/ProImg/"), pre);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = Path.Combine(Server.MapPath("/Images/ProImg/" + pre), fileName);
                        //Tạo đường dẫn tới file
                        //Lưu tên
                        pro.BaseImg = "/Images/ProImg/" + pre + fileName;
                        //Save vào Images Folder
                        pro.UploadImage.SaveAs(path);
                    }
                    db.Products.Add(pro);
                    db.SaveChanges();
                    return RedirectToAction("AdminViewProList", "Product");
                }
            }
            ModelState.AddModelError(string.Empty, "Nhập thông tin không hợp lệ");
            return View();
        }
        public ActionResult Edit(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var pro = db.Products.FirstOrDefault(a => a.ProID == id);
            if (pro != null)
            {
                return View(pro);
            }
            return RedirectToAction("Dashboard", "Admin");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product pro)
        {
            if (ModelState.IsValid)
            {
                var check = db.Products.FirstOrDefault(a => a.ProID == pro.ProID);
                if (check != null)
                {
                    if (pro.UploadImage != null)
                    {
                        var fileName = Path.GetFileName(pro.UploadImage.FileName);
                        string pre = pro.ProName + "/";
                        var path = Path.Combine(Server.MapPath("~/Images/ProImg/"), pre);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = Path.Combine(Server.MapPath("/Images/ProImg/" + pre), fileName);
                        //Tạo đường dẫn tới file
                        //Lưu tên
                        check.BaseImg = "/Images/ProImg/" + pre + fileName;
                        //Save vào Images Folder
                        pro.UploadImage.SaveAs(path);
                    }
                    check.ProName = pro.ProName;
                    check.Price = pro.Price;
                    check.SalePercent = pro.SalePercent;
                    check.CPU = pro.CPU;
                    check.RAM = pro.RAM;
                    check.Capacity = pro.Capacity;
                    check.ProDescription = pro.ProDescription;
                    check.BraID = pro.BraID;
                    check.CatID = pro.CatID;
                    check.Hot = pro.Hot;
                    check.New = pro.New;
                    check.ImgList = pro.ImgList;
                    db.SaveChanges();
                    return RedirectToAction("AdminViewDetailPro", "Product", new { id = pro.ProID });
                }
            }
            return View();
        }
        public ActionResult ProList(int? id, FormCollection f)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            string s = f["s"];
            if (s != "")
            {
                var slist = from c in db.Products
                            where c.ProName.Contains(s)
                            select c;
                return View(slist);
            }
            int x = id ?? 0;
            var list = db.Products.ToList();
            switch (x)
            {
                case 1: //lấy ra danh sách những sản phẩm còn bán (tìm trong danh sách thiết bị hiện có)
                    var res = from c in db.Products
                              where db.Devices.Any(d => d.ProID == c.ProID)
                              select c;
                    return View(res);
                case 2: //danh sách sản phẩm hot
                    res = from c in db.Products
                          where c.Hot == true
                          select c;
                    return View(res);
                case 3: //danh sách sản phẩm mới
                    res = from c in db.Products
                          where c.New == true
                          select c;
                    return View(res);
                case 4: //danh sách sản phẩm vừa mới vừa hot
                    res = from c in db.Products
                          where c.New == true && c.Hot == true
                          select c;
                    return View(list);
                case 5: //danh sách sản phẩm không còn hàng
                    res = from c in db.Products
                          where !db.Devices.Any(d => d.ProID == c.ProID)
                          select c;
                    return View(res);
                default: return View(list);
            }
        }
        public ActionResult Delete(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var pro = db.Products.FirstOrDefault(a => a.ProID == id);
            if (pro == null)
                return RedirectToAction("AdminViewProList", "ViewProduct");
            if (pro.Quantity != 0)
                return RedirectToAction("AdminViewProList", "ViewProduct");
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product pro)
        {
            var product = db.Products.FirstOrDefault(a => a.ProID == pro.ProID);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
            return RedirectToAction("ProList");
        }
        public ActionResult Search()
        {
            return PartialView();
        }
        public ActionResult SaleList()
        {
            var query = from c in db.Products
                        where c.SalePercent >= 10
                        orderby c.SalePercent descending
                        select c;
            var vms = new List<ProductDetailViewModel>();
            foreach (var p in query)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult ListOfBrand(int id)
        {
            var query = from c in db.Products
                        where c.BraID == id
                        select c;
            var vms = new List<ProductDetailViewModel>();
            foreach (var p in query)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult ListOfCate(int id)
        {
            var query = from c in db.Products
                        where c.CatID == id
                        select c;
            var vms = new List<ProductDetailViewModel>();
            foreach (var p in query)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult Products(FormCollection f, int? page)
        {
            //
            //SessionConfig.SetUser(db.Customers.First());
            //
            int pageSize = 6;
            int pageNum = (page ?? 1);

            var list = db.Products.ToList();
            decimal min = 0, max = 99999000;
            int cat = 0, bra = 0;
            if (Session["min"] != null)
            {
                ViewBag.min = Session["min"];
                min = int.Parse(Session["min"].ToString());
            }
            if (!string.IsNullOrEmpty(f["min"]))
            {
                min = decimal.Parse(f["min"].ToString());
                Session["min"] = min;
            }
            ViewBag.min = min;

            if (Session["max"] != null)
            {
                ViewBag.max = Session["max"];
                max = decimal.Parse(Session["max"].ToString());
            }
            else if (!string.IsNullOrEmpty(f["max"]))
            {
                max = 9999999999;
                max = decimal.Parse(f["max"].ToString());
                Session["max"] = max;
            }
            ViewBag.max = max;

            if (Session["bra"] != null)
            {
                bra = int.Parse(Session["bra"].ToString());
            }
            if (!string.IsNullOrEmpty(f["BraID"]))
            {
                bra = int.Parse(f["BraID"].ToString());
                Session["bra"] = bra;
            }


            if (Session["cat"] != null)
            {
                cat = int.Parse(Session["cat"].ToString());
            }
            if (!string.IsNullOrEmpty(f["CatID"]))
            {
                cat = int.Parse(f["CatID"].ToString());
                Session["cat"] = cat;
            }
            list = list.Where(a => (a.Price - a.Price * a.SalePercent / 100) >= min && a.Price - a.Price * a.SalePercent / 100 <= max).ToList();
            if (bra != 0)
            {
                list = list.Where(a => a.BraID == bra).ToList();
            }
            if (cat != 0)
            {
                list = list.Where(a => a.CatID == cat).ToList();
            }

            var vms = new List<ProductDetailViewModel>();
            foreach (var p in list)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms.ToPagedList(pageNum, pageSize));
        }
        public ActionResult ClearSession()
        {
            Session["min"] = null;
            Session["max"] = null;
            Session["bra"] = null;
            Session["cat"] = null;
            return RedirectToAction("Products", "Product");
        }
        public ActionResult SearchList(string s, int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var vms = new List<ProductDetailViewModel>();
            var list = from c in db.Products
                       select c;
            if (!string.IsNullOrEmpty(s))
            {
                //tìm những sản phẩm có tên hoặc phân loại hay hãng giống chuỗi tìm kiếm:
                list = from c in db.Products
                       where c.ProName.Contains(s)
                       || db.Brands.Any(b => b.BraID == c.BraID && b.BraName.Contains(s))
                       || db.Categories.Any(b => b.CatID == c.CatID && b.CatName.Contains(s))
                       select c;
            }
            foreach (var p in list)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            ViewBag.search = s;
            return View(vms.ToPagedList(pageNum, pageSize));
        }
        public ActionResult DetailPro(int id)
        {
            var pro = db.Products.FirstOrDefault(a => a.ProID == id);
            var vm = new ProductDetailViewModel();
            vm.Product = pro;
            return View(vm);
        }
        public ActionResult AdminViewProList()
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var pros = db.Products.ToList();
            var vms = new List<ProductDetailViewModel>();
            foreach (var p in pros)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms);
        }
        public ActionResult AdminViewDetailPro(int id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var pro = db.Products.FirstOrDefault(a => a.ProID == id);
            var vm = new ProductDetailViewModel();
            vm.Product = pro;
            return View(vm);
        }
        public ActionResult List(int? cate, int? bra, int? hot, int? New)
        {
            var list = db.Products.ToList();
            if (cate != null)
                list = list.Where(a => a.CatID == cate).ToList();
            if (bra != null)
                list = list.Where(a => a.BraID == bra).ToList();
            if (hot == 1)
                list = list.Where(a => a.Hot == true).ToList();
            if (New == 1)
                list = list.Where(a => a.New == true).ToList();
            var vms = new List<ProductDetailViewModel>();
            foreach (var p in list)
            {
                var vm = new ProductDetailViewModel();
                vm.Product = p;
                vms.Add(vm);
            }
            return View(vms);
        }
    }
}