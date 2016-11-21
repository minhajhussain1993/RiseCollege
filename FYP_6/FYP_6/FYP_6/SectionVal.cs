using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(SectionVal))]
    public partial class Section
    {

    }
    public class SectionVal
    {
        public SectionVal()
        {
            this.Batches = new HashSet<Batch>();
        }
        public System.Guid SectionID { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Section Name must be Alphabetic")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Section Name must be between 1 and 10 characters")]

        public string SectionName { get; set; }
    
        public virtual ICollection<Batch> Batches { get; set; }
    }
}