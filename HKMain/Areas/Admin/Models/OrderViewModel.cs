using HKShared.Data;
using System.ComponentModel.DataAnnotations;

namespace HKMain.Areas.Admin.Models
{
    public class ShippingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? ZipCode { get; set; } 
        public EntityStatus Status { get; set; }
    }
    public class ShippingEditModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Address { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? ZipCode { get; set; }
        public AppUser? User { get; set; }
    }
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string AvatarUser { get; set; }
        public string NameUser { get; set; }
        public string EmailUser { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public string Method { get; set; }
    }
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public string ImageItem { get; set; }
        public string NameItem { get; set; }
        public string Material { get; set; }
        public double Price { get; set; }
        public int Quantity { get;set; }
        public double Total { get; set; }
    }
    public class OrderEditViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string AvatarUser { get; set; }
        public string NameUser { get; set; }
        public string EmailUser { get; set; }
        public string PhoneUser { get; set; }
        public string ShippingAddress { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus Status { get; set; }
        public string Method { get; set; }
        public virtual ICollection<OrderItemViewModel> Items { get; set; }
    }
}
