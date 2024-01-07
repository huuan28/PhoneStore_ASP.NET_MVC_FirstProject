using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class BuyNow
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        public int CusID { get; set; }
        public int ProID {  get; set; }
        public int Quantity { get; set; }
        public string Color {  get; set; }

    }
}