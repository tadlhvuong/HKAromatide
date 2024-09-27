using System.ComponentModel.DataAnnotations;

namespace HKShared.Data
{
    public class AppSetting
    {
        [Key]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
