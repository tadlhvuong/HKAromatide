using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKShared.Data
{
    public class ProductTaxo
    {
        public int Id { get; set; }

        public int TaxoId { get; set; }

        public int ItemId { get; set; }
        public TaxoType ItemType { get; set; }

        [ForeignKey("TaxoId")]
        public virtual Taxonomy Taxonomy { get; set; }

        [ForeignKey("ItemId")]
        public virtual Product Products { get; set; }
    }
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "UserId")]
        public string? UserId { get; set; }
        [Display(Name = "AlbumId")]
        public int? AlbumId { get; set; }

        [Display(Name = "CategoryId")]
        public int? CategoryId { get; set; }

        [StringLength(64)]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = "";

        [StringLength(512)]
        [Display(Name = "Hình đại diện")]
        public string Image { get; set; } = "~/images/image-default.png";

        [Required, StringLength(128)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(1024)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Preview")]
        public string Preview { get; set; } = "";

        [DataType(DataType.MultilineText)]
        [Display(Name = "Content")]
        public string Content { get; set; } = "";

        [StringLength(128)]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:#,#} VNĐ")]
        public double Price { get; set; } = 0;

        [Display(Name = "SalePrice")]
        [DisplayFormat(DataFormatString = "{0:#,#} VNĐ")]
        public double SalePrice { get; set; } = 0;

        [Display(Name = "Quantity")]
        public long Quantity { get; set; } = 0;

        [Display(Name = "Stock")]
        public bool Stock { get; set; } = false;

        [Display(Name = "Tax")]
        public float Tax { get; set; }

        [Display(Name = "Tags")]
        public string? Tags { get; set; }

        [StringLength(256)]
        [Display(Name = "Status")]
        [DataType(DataType.MultilineText)]
        public ProductStatus Status { get; set; }

        [Display(Name = "CreateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "CreateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? LastTime { get; set; }

        [Display(Name = "Phân loại")]
        public virtual ICollection<ProductTaxo> Taxonomies { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }
        [ForeignKey("AlbumId")]
        public virtual MediaAlbum MediaAlbum { get; set; }

        [NotMapped]
        public virtual ICollection<MediaFile>? MediaFiles { get; set; }
        [NotMapped]
        public virtual string? LinkImage { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:#,#.000} VNĐ")]
        public virtual double CurrentPrice
        {
            get
            {
                if (SalePrice > 0)
                    return SalePrice;

                return Price;
            }
        }


        [NotMapped]
        public virtual bool NewProduct
        {
            get
            {
                if (CreateTime == null)
                    return false;

                var diff = DateTime.Now - CreateTime.Value;
                return (diff.TotalHours < 168);
            }
        }

        [NotMapped]
        public virtual string DiscountPercent
        {
            get
            {
                if (this.SalePrice != this.Price && this.SalePrice > 0)
                {
                    double number = (this.Price - this.SalePrice) / this.Price;
                    return Convert.ToDecimal(number).ToString("#.#%");
                }
                else
                {
                    return "";
                }
            }
        }

        [NotMapped]
        public virtual string FormatedCurrentPrice
        {
            get
            {
                return Convert.ToDecimal(this.CurrentPrice).ToString("#,#đ");
            }
        }

        [NotMapped]
        public virtual string FormatedSalePrice
        {
            get
            {
                if (this.SalePrice == 0) return "0đ";
                return Convert.ToDecimal(this.SalePrice).ToString("#,#đ");
            }
        }

        [NotMapped]
        public virtual string FormatedRegularPrice
        {
            get
            {
                if(this.Price == 0) return "0đ";
                return Convert.ToDecimal(this.Price).ToString("#,#đ");
            }
        }
    }
}
