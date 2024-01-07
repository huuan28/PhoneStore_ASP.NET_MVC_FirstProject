using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using DAWebPhone.App_Start;

namespace DAWebPhone.Controllers
{

    public class HomeController : Controller
    {
        DBWebPhoneEntities db=new DBWebPhoneEntities();
        public ActionResult Index()
        {
            //SessionConfig.SetUser(db.Customers.First());
            return View();
        }
        public ActionResult Top6Hot()
        {
            var list = from c in db.Products
                       where c.Hot == true
                       orderby c.Price descending
                       select c;
            return PartialView(list.Take(6).ToList());
        }
        public ActionResult Top6New()
        {
            var list = from c in db.Products
                       where c.New == true
                       orderby c.Price descending
                       select c;
            return PartialView(list.Take(6).ToList());
        }
        public ActionResult BestSeller()
        {
            var list = from p in db.Products
                        where (from d in db.Devices
                               where d.DvStatus == 1
                               group d by d.ProID into g
                               orderby g.Count() descending
                               select g.Key).Take(6).Contains(p.ProID)
                        select p;
            return PartialView(list.ToList());
        }
        public ActionResult TopSale()
        {
            var list = from p in db.Products
                       orderby p.SalePercent descending
                       select p;
            return PartialView(list.Take(6).ToList());
        }
        public ActionResult BigSale()
        {
            var list = from c in db.Products
                       orderby c.SalePercent descending
                       select c;
            return PartialView(list.Take(6).ToList());
        }
        public ActionResult About()
        {
            var x = db.Customers.ToList();

            return View(x);
        }
        public ActionResult UpLinkImg()
        {
            var x = db.Products.ToList();
            foreach (var pro in x)
            {
                pro.ImgList = null;
                var s = pro.BaseImg.Split('/');
                for (int i = 0; i < s.Length - 1; i++)
                {
                    pro.ImgList += s[i]+"/";
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult notfound()
        {
            return HttpNotFound();
        }
    }
}