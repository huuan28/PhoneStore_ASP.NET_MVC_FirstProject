using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class OrderViewModel
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        public OrderPro OrderPro { get; set; }
        public Customer Customer => db.Customers.FirstOrDefault(a=>a.CusID == OrderPro.CusID);
        public AdminUser AdminUser => db.AdminUsers.FirstOrDefault(a=>a.AdmID== OrderPro.Handler);
        public List<OrderDetailViewModel> OrderDetail
        {
            get
            {
                var list = new List<OrderDetailViewModel>();
                var ods = db.OrderDetails.Where(a => a.OrdID == OrderPro.OrdID).ToList();
                foreach (var item in ods)
                {
                    var x = new OrderDetailViewModel();
                    x.OrderDetail = item;
                    list.Add(x);
                }
                return list;
            }
        }
        public string PayMethod => OrderPro.PayMethod == 0 ? "Ship COD" : "Tài khoản";
        public string Price => $"{OrderPro.TotalPrice:#,0} đ";
        public string Status
        {
            get
            {
                switch (OrderPro.OrdStatus)
                {
                    case 0: return "Chưa đặt";
                    case 1: return "Chờ xử lý";
                    case 2: return "Đã duyệt";
                    case 3: return "Hủy";
                    case 4: return "Bị Hủy";
                    default: return "-";
                }
            }
        }
    }
}