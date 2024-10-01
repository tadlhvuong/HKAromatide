using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using HKShared.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HKMain.Models
{
    public class SubscriptionModel
    {
        public string? emailsend { get; set; }
    }
    
    public class MegaMenuModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string RedirectUrl { get; set; }
        public string[] Taxo{ get; set; }
    }
    public class ShopCat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
    }
    public class ShopVariantDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ShopVariantDetails>? VariantChild { get; set; }
    }
    public class CheckoutItem
    {
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Product_Image { get; set; }
        public int Product_Quantity { get; set; }
        public double Product_Price { get; set; }
        public string Product_Variant { get; set; }
        public string NameParent { get; set; }
        public string NameChild { get; set; }
        public string UrlReturn { get; set; }

    }

    public class Checkout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int Payment { get; set; }
        public bool isAgressPrivacy { get; set; } = false;
        public string? Items { get; set; }
    }
}
