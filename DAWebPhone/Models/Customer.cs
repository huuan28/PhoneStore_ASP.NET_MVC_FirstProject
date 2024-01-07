﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAWebPhone.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Carts = new HashSet<Cart>();
            this.OrderProes = new HashSet<OrderPro>();
            CusCreDate = DateTime.Now;
            CusAvatar = "~/Images/CusAvt/avt.png";
        }

        [Display(Name = "Mã tài khoản")]
        public int CusID { get; set; }

        [Required(ErrorMessage = "Chưa điền tên đăng nhập!")]
        [Display(Name = "Tên đăng nhập")]
        [MinLength(6, ErrorMessage = "Tên đăng nhập phải có ít nhất 6 kí tự")]
        public string CusLogName { get; set; }

        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Chưa điền họ tên!")]
        public string CusName { get; set; }


        [Required(ErrorMessage = "Chưa điền mật khẩu!")]
        [Display(Name = "Mật khẩu")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Mật khẩu phải trên 8 kí tự và có ít nhất 1 chữ hoa, 1 chữ thường, 1 số, 1 kí tự đặc biệt")]
        [DataType(DataType.Password)]
        public string CusPass { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "Chưa xác nhận mật khẩu!")]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("CusPass", ErrorMessage = "Xác nhận mật khẩu không trùng khớp!")]
        [DataType(DataType.Password)]
        public string ConfirmPass { get; set; }

        [Required(ErrorMessage = "Chưa điền Email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không đúng định dạng")]
        public string CusEmail { get; set; }

        [Required(ErrorMessage = "Chưa điền số điện thoại!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [Display(Name = "Số điện thoại")]
        public string CusPhone { get; set; }

        [Required(ErrorMessage = "Chưa chọn giới tính!")]
        [Display(Name = "Giới tính")]
        public int CusGender { get; set; }

        [Required(ErrorMessage = "Chưa nhập ngày sinh!")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public System.DateTime CusBirth { get; set; }

        [Required(ErrorMessage = "Chưa điền địa chỉ!")]
        [Display(Name = "Địa chỉ")]
        public string CusAddress { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string CusAvatar { get; set; }

        [Display(Name = "Số dư")]
        public Nullable<decimal> Credit { get; set; }


        public Nullable<int> VIP { get; set; }


        [Display(Name = "Trạng thái hoạt động")]
        public Nullable<int> CusStatus { get; set; }

        [Display(Name = "Kích hoạt")]
        public Nullable<bool> CusActive { get; set; }

        [Display(Name = "Ngày tạo tài khoản")]
        public Nullable<System.DateTime> CusCreDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderPro> OrderProes { get; set; }
        [NotMapped]
        public HttpPostedFileBase UploadImage { get; set; }
        [NotMapped]
        public string Sodu => $"{Credit:#,0} ₫";
        [NotMapped]
        public string GoiTinh
        {
            get
            {
                switch (CusGender)
                {
                    case 0: return "Nam";
                    case 1: return "Nữ";
                    default: return "Khác";
                }
            }
        }

    }
}