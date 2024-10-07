using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKShared.Data
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ItemId { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Thuộc tính")]
        public string ItemAttrib { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order ShopOrder { get; set; }

        [ForeignKey("ItemId")]
        public virtual Product Product { get; set; }

        [NotMapped]
        public virtual string SubTotal
        {
            get
            {
                double number = this.Quantity * this.Product.CurrentPrice;
                return Convert.ToDecimal(number).ToString("#,### đ");
            }
        }
    }

    public class Order
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }

        [Display(Name = "Giao hàng")]
        public string Address { get; set; }

        [Display(Name = "chiết khấu")]
        public double AdjustPrice { get; set; }

        [Display(Name = "Phí giao hàng")]
        public double ShippingFee { get; set; }

        [Display(Name = "Tổng cộng")]
        [DisplayFormat(DataFormatString = "{0:#,#} VNĐ")]
        public double GrandTotalPrice { get; set; }

        [Display(Name = "Thanh toán")]
        public string PaymentInfo { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "Ngày giao")]
        public DateTime ShippingTime  { get; set; } = DateTime.Now;

        [StringLength(256)]
        [Display(Name = "Ghi chú")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        [Display(Name = "Trạng thái Đơn hàng")]
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Trạng thái Thanh toán")]
        public PaymentStatus PaymentStatus { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        [Display(Name = "Sản phẩm")]
        public virtual ICollection<OrderItem> Items { get; set; }
        public bool IsAgree { get; set; } = false;

        [NotMapped]
        public virtual string FormatedGrandTotalPrice
        {
            get
            {
                return Convert.ToDecimal(this.GrandTotalPrice).ToString("#,### đ");
            }
        }
    }
}
