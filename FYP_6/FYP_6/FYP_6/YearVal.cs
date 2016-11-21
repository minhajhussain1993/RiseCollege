using FYP_6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(YearVal))]
    public partial class Year
    {

    }
    public class YearVal
    {
        public YearVal()
        {
            this.Batches = new HashSet<Batch>();
        }

        public Guid YearID { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Starting Year must be a Number")]

        public Nullable<int> FromYear { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Ending Year must be a Number")]

        public Nullable<int> ToYear { get; set; }
    
        public virtual ICollection<Batch> Batches { get; set; }
    }
}