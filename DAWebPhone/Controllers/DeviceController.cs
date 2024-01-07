using DAWebPhone.App_Start;
using DAWebPhone.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAWebPhone.Controllers
{
    public class DeviceController : Controller
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        // GET: Device
        public ActionResult Import()
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var dv = new Device();
            return View(dv);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Device dv)
        {
            if (ModelState.IsValid)
            {
                dv.ImportDate = DateTime.Now;
                db.Devices.Add(dv);
                db.SaveChanges();
            }
            return RedirectToAction("DeviList");
        }
        public ActionResult Search(FormCollection form)
        {
            if (form["ProID"]==null)
                return RedirectToAction("DeviList");
            int id = int.Parse(form["ProID"]);
            return RedirectToAction("DeviceList", new {id = id});
        }
        public ActionResult Edit(string code)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var dv = db.Devices.FirstOrDefault(a=>a.Code == code);
            if (dv != null)
            {
                return View(dv);
            }
            return RedirectToAction("DeviceList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Device dv)
        {
            if (ModelState.IsValid)
            {
                var dev = db.Devices.FirstOrDefault(a=>a.Code==dv.Code);
                if(dev != null)
                {
                    dev.ProID = dv.ProID;
                    dev.DvStatus = dv.DvStatus;
                    dev.CreatedDate = dv.CreatedDate;
                    dev.ImportDate = dv.ImportDate;
                    dev.Note = dv.Note;
                    db.SaveChanges();
                    return RedirectToAction("DeviceList");
                }
            }
            ModelState.AddModelError("", "Thông tin không hợp lệ");
            return View();
        }
        public ActionResult Delete(string code)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var dv = db.Devices.FirstOrDefault(a => a.Code == code);
            if (dv != null)
            {
                return View(dv);
            }
            return RedirectToAction("DeviceList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Device dv)
        {
            var dev = db.Devices.FirstOrDefault(a=>a.Code==dv.Code);
            db.Devices.Remove(dv);
            db.SaveChanges();
            return RedirectToAction("DeviceList");
        }
        public ActionResult DeviceList(int? id)
        {
            if (SessionConfig.GetAdm() == null)
                return RedirectToAction("Login", "Admin");
            var vms = new List<DeviceDetailViewModel>();
            var ds = db.Devices.ToList();
            if(id!=null)
                ds = db.Devices.Where(a=>a.ProID==id).ToList();
            foreach (var i in ds)
            {
                var vm = new DeviceDetailViewModel();
                vm.Device = i;
                vms.Add(vm);
            }
            return View(vms);
        }
    }
}