using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HKShared.Data
{
    public class AccountLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Action")]
        public UserAction Action { get; set; }

        [Display(Name = "IP")]
        [StringLength(60)]
        public string RemoteIP { get; set; }

        [Display(Name = "LogData")]
        public string LogData { get; set; }

        [Display(Name = "CreateTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime LogTime { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }
    }
}
