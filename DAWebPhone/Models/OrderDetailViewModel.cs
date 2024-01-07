using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class OrderDetailViewModel
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        public OrderDetail OrderDetail { get; set; }
        public Device Device => db.Devices.FirstOrDefault(a=>a.Code==OrderDetail.Code);
        public Product Product => db.Products.FirstOrDefault(a=>a.ProID==Device.ProID);
        public string Color
        {
            get
            {
                switch (Device.Color.ToLower())
                {
                    case "black": return "Đen";
                    case "white": return "Trắng";
                    case "blue": return "Xanh";
                    case "gold": return "Vàng";
                    case "purpose": return "Tím";
                    default: return "";
                }
            }
        }
        public string Price => $"{Product.Price:#,0} đ";
        public string FinalPrice => $"{Product.FinalPrice:#,0} đ";
    }
}