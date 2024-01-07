using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAWebPhone.Models
{
    public class ProductDetailViewModel
    {
        DBWebPhoneEntities db = new DBWebPhoneEntities();
        public Product Product { get; set; }
        public Brand Brand => db.Brands.FirstOrDefault(a => a.BraID == Product.BraID);        
        public Category Category => db.Categories.FirstOrDefault(a => a.CatID == Product.CatID);
        public List<ImagesPro> ImagesPros => db.ImagesProes.Where(a => a.ProID == Product.ProID).ToList();
        public string Price => $"{Product.Price:#,0} ₫";
        public string Salepercent => $"-{Product.SalePercent}%";
        public string FinalPrice => $"{Product.FinalPrice:#,0} ₫";
    }
}