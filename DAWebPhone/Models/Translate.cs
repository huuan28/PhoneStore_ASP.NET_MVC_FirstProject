using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public static class Translate
    {
        public static string Color(string cl)
        {
            string res = cl.ToLower();
            switch (res)
            {
                case "white": return "Trắng";
                case "black": return "Đen";
                case "blue": return "Xanh";
                case "puple": return "Tím";
                case "gold": return "Vàng";
                case "grey": return "Xám";
                default: return cl;
            }
        }
        public static string DvStatus(int? stt)
        {
            switch (stt)
            {
                case 0: return "Chưa bán";
                case 1: return "Đang đặt";
                case 2: return "Đã bán";
                case 3: return "Lỗi";
                case 4: return "Hết bảo hành";
                default: return stt.ToString();
            }
        }
        public static string OrdStatus(int? stt)
        {
            switch (stt)
            {
                case 0: return "Chưa đặt";
                case 1: return "Chờ xử lý";
                case 2: return "Đã chấp thuận";
                case 3: return "Khách hủy";
                case 4: return "Hủy";
                default: return stt.ToString();
            }
        }
    }
}