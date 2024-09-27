using HKShared.Data;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HKMain.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string AvatarImg { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string? RoleName { get; set; }
        public string[] Roles { get; set; }
        public EntityStatus Status { get; set; }
        public ResetPasswordViewModel ResetPasswordModel { get; set; }
    }

    public class EditUserModel
    {
        [Required(ErrorMessage = "{0} not null")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "{0} not null")]
        [Display(Name = "Tên")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "{0} not null")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "{0} invalid")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Trạng thái")]
        public EntityStatus Status { get; set; }

        public ModalFormResult? modelResult { get; set; }
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} not null")]
        [EmailAddress(ErrorMessage = "{0} invalid")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} not null")]
        [StringLength(100, ErrorMessage = "{0} phải có độ dài ít nhât {2} kí tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} not null")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "NotMatchConfirmPass")]
        public string ConfirmPassword { get; set; }
    }
    public class MemberSearchModel
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Display(Name = "IdCCCD")]
        public string IdCardNo { get; set; }

        [Display(Name = "FindMode")]
        public int FindMode { get; set; }

        [Display(Name = "Kết quả")]
        public List<AppUser> Results { get; set; }
    }
}
