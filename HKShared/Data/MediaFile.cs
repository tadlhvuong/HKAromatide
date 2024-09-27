using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKShared.Data
{
    public class MediaFile
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }

        [StringLength(128)]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [StringLength(512)]
        [Display(Name = "FullPath")]
        public string FullPath { get; set; }

        [Display(Name = "FileSize")]
        public long FileSize { get; set; }

        [Display(Name = "CreateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime? CreateTime { get; set; }

        [ForeignKey("AlbumId")]
        public virtual MediaAlbum MediaAlbum { get; set; }

        [NotMapped]
        [StringLength(512)]
        [Display(Name = "URL tải file")]
        public string FileLink { get; set; }
    }
}
