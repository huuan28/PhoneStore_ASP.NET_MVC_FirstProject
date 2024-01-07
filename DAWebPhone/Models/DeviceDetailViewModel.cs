using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class DeviceDetailViewModel
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        public Device Device { get; set; }
        public Product Product => db.Products.FirstOrDefault(a=>a.ProID==Device.ProID);
        public string Color
        {
            get
            {
                string res = Device.Color.ToLower();
                switch (res)
                {
                    case "white": return "Trắng";
                    case "black": return "Đen";
                    case "blue": return "Xanh";
                    case "gold": return "Vàng";
                    case "grey": return "Xám";
                    case "purpose": return "tím";
                    default: return Device.Color;
                }
            }
        }
        public string Status
        {
            get
            {
                switch (Device.DvStatus)
                {
                    case 0: return "Chưa đặt";
                    case 1: return "Chờ xử lý";
                    case 2: return "Đã bán";
                    case 3: return "Lỗi";
                    default: return "Hết bảo hành";
                }
            }
        }
    }
}