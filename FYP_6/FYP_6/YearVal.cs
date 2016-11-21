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
        [Required(ErrorMessage = "Starting Year is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Starting Year must be a Year Value")]
        [Range(1700, 9999, ErrorMessage = "Plz Enter A Valid Year")]
        public Nullable<int> FromYear { get; set; }
        [Required(ErrorMessage = "Ending Year is required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Ending Year must be a Year Value")]
        [Range(1700, 9999, ErrorMessage = "Plz Enter A Valid Year")]
        public Nullable<int> ToYear { get; set; }
    
        public virtual ICollection<Batch> Batches { get; set; }
    }
}