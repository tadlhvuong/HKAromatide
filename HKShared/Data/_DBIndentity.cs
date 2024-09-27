using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKShared.Data
{
    public class AppRole : IdentityRole
    {
        public AppRole()
        {
        }

        public AppRole(string roleName)
            : base(roleName)
        {
        }
    }

    public class AppUser : IdentityUser
    {
        [Display(Name = "Đã khóa")]
        public bool IsLocked { get; set; }

        [Display(Name = "Thời gian mở")]
        public DateTime? OpenTime { get; set; }

        [Display(Name = "Status")]
        public EntityStatus Status { get; set; }

        [Display(Name = "CreateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [Display(Name = "IPCreate")]
        [StringLength(60)]
        public string? CreateIP { get; set; }

        [Display(Name = "LastLogin")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "IPLogin")]
        [StringLength(60)]
        public string? LastLoginIP { get; set; }

        [Display(Name = "LastUpdate")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? LastUpdate { get; set; }

        [StringLength(128)]
        [Display(Name = "UpdateUser")]
        public string? UpdateUser { get; set; }
        [NotMapped]
        public UserMode UserMode
        {
            get
            {
                if (!EmailConfirmed)
                    return UserMode.None;

                if (!IsLocked)
                    return UserMode.Unlocked;

                if (OpenTime == null)
                    return UserMode.Locked;

                if (OpenTime > DateTime.Now)
                    return UserMode.Unlocking;
                else
                    return UserMode.Unlocked;
            }
        }

        [NotMapped]
        public virtual ICollection<UserLoginInfo> ExtLogins { get; set; }

        [NotMapped]
        public string AvatarImg
        {
            get
            {
                if (ExtLogins == null)
                    return "/images/admin/1.jpg";

                var fbLogin = ExtLogins.FirstOrDefault(x => x.LoginProvider == "Facebook");
                if (fbLogin == null)
                    return "/images/admin/1.jpg";

                return string.Format("https://graph.facebook.com/{0}/picture?type=large", fbLogin.ProviderKey);
            }
        }

        [NotMapped]
        public string FacebookUrl
        {
            get
            {
                if (ExtLogins == null)
                    return null;

                var fbLogin = ExtLogins.FirstOrDefault(x => x.LoginProvider == "Facebook");
                if (fbLogin == null)
                    return null;

                return string.Format("https://www.facebook.com/app_scoped_user_id/{0}", fbLogin.ProviderKey);
            }
        }
    }
}
