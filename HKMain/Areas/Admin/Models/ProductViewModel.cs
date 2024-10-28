using HKShared.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HKMain.Areas.Admin.Models
{
    public class ProductFormModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }
        public int? IdAlbum { get; set; }
        public string? IdFiles { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        public string SKU { get; set; }
        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Miêu tả")]
        public string Preview { get; set; } = "";
        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Nội dung")]
        public string Content { get; set; } = "";
        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Số lượng")]
        public long Quantity { get; set; } = 0;
        [DisplayName("Đơn giá (VNĐ)")]
        public Double Price { get; set; } = 0.0;
        [DisplayName("Giá giảm (VNĐ)")]
        public Double SalePrice { get; set; } = 0.0;
        public List<MediaFile> MediaFile { get; set; }
        [DisplayName("Tag")]
        public string? Tags { get; set; } = "";
        [DisplayName("Thuế")]
        public long Tax { get; set; }
        [DisplayName("SL Tồn kho")]
        public bool Stock { get; set; } = false;
        [DisplayName("Chọn phân loại")]
        public int ItemCategory { get; set; }
        [DisplayName("Chọn bộ sưu tập")]
        public int ItemCollection { get; set; }
        [DisplayName("Chọn nhà phân phối")]
        public int ItemVendor { get; set; }
        [DisplayName("Trạng thái")]
        public ProductStatus Status { get; set; }
        public string Attributes { get; set; }
    }

    public class ProductViewModel
    {
        public int IdProduct { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string? Category { get; set; }
        public string? Collection { get; set; }
        public string? Vendor { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public bool Stock {  get; set; }
        public long Quantity { get; set; }
        public ProductStatus Status { get; set; }
    }
    public class ProductAttrModel
    {
        public int Parent { get; set; }
        public int Child { get; set; }
        public int Price { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
    }
}
