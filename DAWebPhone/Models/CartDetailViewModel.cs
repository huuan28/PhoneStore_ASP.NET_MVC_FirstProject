using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class CartDetailViewModel
    {
        DBWebPhoneEntities db  = new DBWebPhoneEntities();
        public Cart Cart { get; set; }
        public Product Product => db.Products.FirstOrDefault(a=>a.ProID==Cart.ProID);
        public Customer Customer => db.Customers.FirstOrDefault(a=>a.CusID==Cart.CusID);
        public decimal TotalPrice => Product.FinalPrice * (int)Cart.Quantity;
        public string ShowPrice => $"{TotalPrice:#,0} ₫";
    }
}