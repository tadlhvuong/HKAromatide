using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HKShared.Data
{
    public class MediaAlbum
    {
        public int Id { get; set; }

        [Display(Name = "Người tạo")]
        public string? UserId { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Tên Album")]
        public string ShortName { get; set; }

        [Required, StringLength(128)]
        [Display(Name = "Tên đầy đủ")]
        public string FullName { get; set; }

        [StringLength(256)]
        [Display(Name = "Miêu tả")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? AppUser { get; set; }

        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [NotMapped]
        public virtual ICollection<MediaFile>? MediaFiles { get; set; }
    }
}
