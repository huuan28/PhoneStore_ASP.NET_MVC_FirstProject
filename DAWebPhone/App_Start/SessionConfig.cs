using DAWebPhone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.App_Start
{
    public class SessionConfig
    {
        public static void SetAdm(AdminUser adm)
        {
            HttpContext.Current.Session["adm"] = adm;
        }
        public static AdminUser GetAdm()
        {
            return (AdminUser)HttpContext.Current.Session["adm"];
        }
        public static void SetUser(Customer cus)
        {
            HttpContext.Current.Session["user"] = cus;
        }
        public static Customer GetUser()
        {
            return (Customer)HttpContext.Current.Session["user"];
        }
        public static void SetCart(Cart cart)
        {
            HttpContext.Current.Session["cart"] = cart;
        }
        public static Cart GetCart()
        {
            return (Cart)HttpContext.Current.Session["cart"];
        }
    }
}