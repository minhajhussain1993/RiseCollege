using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP_6
{
    [MetadataType(typeof(OverAllFeeVal))]
    public partial class Overall_Fees
    {
    }
    public class OverAllFeeVal
    {
        public OverAllFeeVal()
        {
            this.Fees = new HashSet<Fee>();
        } 
        //[Remote("ValidateRollnoForFee", "Home", ErrorMessage = "Roll no Doesnot Exists")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Roll no is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Roll no must be 3 to 30 characters")]
        public string RollNo { get; set; }
         
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Total Degree Fee must be an unsigned whole Number")]
        [Range(0, 100000, ErrorMessage = "Total Degree Fee must be between 0 and 100000")] 
        public Nullable<decimal> Total_Degree_Fee { get; set; }
        //[Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Submitted Fee must be an unsigned whole Number")]
        [Range(0, 100000, ErrorMessage = "Submitted Fee must be between 0 and 100000")] 
        public Nullable<decimal> SubmittedFee { get; set; }
        //[Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Remaining Fee must be an unsigned whole Number")]
        [Range(0, 100000, ErrorMessage = "Remaining Fee must be between 0 and 100000")] 
        public Nullable<decimal> RemainingFee { get; set; }
     //   [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Total Installments must be an unsigned Whole Number")]
         
        [Range(1,10, ErrorMessage = "Total Installments must be between 1 and 10")]
        public Nullable<int> Total_Installments { get; set; }
       // [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Paid Installments must be an unsigned Whole Number")]
        [Range(0, 10, ErrorMessage = "Paid Installments must be between 1 and 10")]
        public Nullable<int> Paid_Installments { get; set; }

        public virtual ICollection<Fee> Fees { get; set; }
        public virtual Registeration Registeration { get; set; }
    }
}