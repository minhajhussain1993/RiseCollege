using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP_6
{
    [MetadataType(typeof(FeeVal))]
    public partial class Fee
    {
    }
    public class FeeVal
    {
        [Required]
        [Remote("ValidateBillNo", "Home", ErrorMessage = "bill no Already Exists")]
        public string Bill_No { get; set; }
         

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Dated { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Registeration_Fee must be an unsigned whole Number")]
        [Range(0, 20000)]
        public Nullable<decimal> Registeration_Fee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tution fee is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Tution_Fee must be an unsigned whole Number")]
        [Range(0, 50000)]
        public Nullable<decimal> Tution_Fee { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Admission_Fee must be an unsigned whole Number")]
        [Range(0, 20000)]
        public Nullable<decimal> Admission_Fee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Exam_Fee is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Exam_Fee must be an whole unsigned Number")]
        [Range(0, 20000)]
        public Nullable<decimal> Exam_Fee { get; set; }
         
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Installments must be an unsigned whole Number")]
        [Range(1,10)]
        public Nullable<int> Installment { get; set; }

        public string Month { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Fine Fee must be an unsigned whole Number")]
        [Range(0, 50000)]
        public Nullable<decimal> Fine { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Total_Fee is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Total_Fee must be an unsigned whole Number")]
        [Range(0, 100000)]
        public Nullable<decimal> Total_Fee { get; set; }
        public string RollNo { get; set; }

        public virtual Overall_Fees Overall_Fees { get; set; }
    }
}