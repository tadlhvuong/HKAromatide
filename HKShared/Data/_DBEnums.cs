using System.ComponentModel.DataAnnotations;
using HKShared.Helpers;

namespace HKShared.Data
{
    public enum EntityStatus
    {
        [Display(Name = "Không rõ"), StatusCss("default")]
        None,
        [Display(Name = "Hoạt động"), StatusCss("success")]
        Enabled,
        [Display(Name = "Tạm ngưng"), StatusCss("warning")]
        Disabled,
        [Display(Name = "Hết hạn"), StatusCss("danger")]
        Expiried,
        [Display(Name = "Thử nghiệm"), StatusCss("info")]
        Testing,
    }

    public enum UserMode
    {
        None,
        Locked,
        Unlocked,
        Unlocking,
    }
    public enum UserAction
    {
        None,
        Login,
        Register,
        ChangePass,
        ChangeEmail,
        VerifyEmail,
        ChangePhone,
        VerifyPhone,
        RequestOTP,
        ExternalAddLogin,
        ExternalRemoveLogin,
        ExternalSetPassword,
        CreateDate,
        UpdateDate,
        RemoveData,
        LockAccount,
        UnlockAccount,
        UnlockAccountDone,
        ForgotPassword,
        ResetPassByEmail,
        ResetPassByPhone,
        Logout,
    }
    public enum AlbumDefault
    {
        Default = 1,
        Banner,
        Intro,
    }

    public enum TaxoType
    {
        Default,
        Album,
        Category,
        Collection,
        Vendor,
        Tags,
        Variants,
        Attributes,
        Shipping,
        Campaign
    }

    public enum EventType
    {
        Drag,
        Main
    }
    public enum ProductMode
    {
        Empty,
        Available
    }
    public enum ProductStatus
    {
        Inactive,
        Scheduled,
        Published
    }
    public enum OrderStatus
    {
        [Display(Name = "Đang treo"), StatusCss("default")]
        Pending,

        [Display(Name = "Đang xử lý"), StatusCss("warning")]
        Processing,

        [Display(Name = "Đang giao hàng"), StatusCss("info")]
        Delivering,

        [Display(Name = "Đã giao hàng"), StatusCss("success")]
        Delivered,

        [Display(Name = "Đã hủy bỏ"), StatusCss("danger")]
        Canceled,
    }
    public enum PaymentStatus
    {
        [Display(Name = "Chưa thanh toán"), StatusCss("default")]
        None,

        [Display(Name = "Một phần"), StatusCss("purple")]
        Partly,

        [Display(Name = "Thanh toán đủ"), StatusCss("success")]
        Fully,

        [Display(Name = "Lỗi thanh toán"), StatusCss("danger")]
        Failed,

        [Display(Name = "Đã hoàn tiền"), StatusCss("info")]
        Refunded,
    }
    public enum NotificationType
    {
        Subscription,
        NewUser,
        NewMessage,
        NewReview,
        ProductPublished,
        BlogPublished
    }
    public enum NotificationStatus
    {
        None,
        UnSend,
        Sending,
        Send,
        Failed
    }
    public enum MessageType
    {
        ChatBox,
        Product,
    }
    public enum SettingMethodContact
    {
        Email,
        SMS
    }
    public enum SettingCustomerName
    {
        LastName,
        FullName
    }
    public enum SettingOption
    {
        None,
        Optional,
        Required
    }
    public enum Unit
    {
        Currency,
        Weight
    }
}
