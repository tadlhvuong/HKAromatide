using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace HKShared.Data
{
    public class Taxonomy
    {
        public int Id { get; set; }

        [Display(Name = "ParentId")]
        public int? ParentId { get; set; }

        [Display(Name = "Type")]
        public TaxoType Type { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [ForeignKey("ParentId")]
        public virtual Taxonomy Parent { get; set; }

        public virtual ICollection<Taxonomy> Children {  get; set; }

        [NotMapped]
        public virtual string ParentName
        {
            get
            {
                if (ParentId == null)
                    return "NULL";
                if (Parent == null)
                    return "NULL";

                return Parent.Name;
            }
        }
    }
}
