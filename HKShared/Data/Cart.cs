using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKShared.Data
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ItemId { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Thuộc tính")]
        public string ItemAttrib { get; set; }

        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        [ForeignKey("ItemId")]
        public virtual Product ShopItem { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public double ItemTotal
        {
            get
            {
                return Quantity * ShopItem.CurrentPrice;
            }
        }

        [NotMapped]
        public string ItemImage
        {
            get
            {
                if (string.IsNullOrEmpty(ItemAttrib))
                    return null;

                try
                {
                    var attValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(ItemAttrib);
                    if (!attValue.ContainsKey("Image"))
                        return null;

                    return attValue["Image"];
                }
                catch
                {
                    return null;
                }
            }
        }

        //[NotMapped]
        //public IHtmlContent ItemAttribDesc
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(ItemAttrib))
        //            return null;

        //        try
        //        {
        //            string attDesc = "";
        //            var attValue = JsonConvert.DeserializeObject<Dictionary<string, string>>(ItemAttrib);
        //            foreach (var key in attValue.Keys)
        //            {
        //                if (key == "Image")
        //                    continue;

        //                attDesc += string.Format("{0}: {1}\n", key, attValue[key]);
        //            }

        //            return HtmlString(attDesc);
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}
    }

    public class Cart
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [Display(Name = "Sản phẩm")]
        public virtual ICollection<CartItem> Items { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public double SubTotal
        {
            get
            {
                return Items != null ? Items.Sum(x => x.ItemTotal) : 0.0;
            }
        }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public double ShippingFee
        {
            get
            {
                if (this.SubTotal < 400000)
                {
                    return 20000;
                }
                else { return 0; }
            }
        }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public double GrandTotal
        {
            get
            {
                return SubTotal + ShippingFee;
            }
        }
    }
}
