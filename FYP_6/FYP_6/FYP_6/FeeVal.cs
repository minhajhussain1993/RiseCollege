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
        [Remote("ValidateRollnoForFee", "Home", ErrorMessage = "Roll no Doesnot Exists")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Roll no is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Roll no must be 3 to 30 characters")]
        public string Rollno { get; set; }
        public string StudentName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> Dated { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Registeration_Fee must be a Number")]
        public Nullable<decimal> Registeration_Fee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tution fee is required")]
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Tution_Fee must be a Number")]
        public Nullable<decimal> Tution_Fee { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Admission_Fee must be a Number")]
        public Nullable<decimal> Admission_Fee { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Exam_Fee is required")]
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Exam_Fee must be a Number")]
        public Nullable<decimal> Exam_Fee { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Total_Installments must be a Number")]
        public Nullable<int> Total_Installments { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Installments must be a Number")]
        public Nullable<int> Installment { get; set; }

        public string Month { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Fine Fee must be a Number")]
        public Nullable<decimal> Fine { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Total_Fee is required")]
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Total_Fee must be a Number")]
        public Nullable<decimal> Total_Fee { get; set; }

        public virtual Registeration Registeration { get; set; }
    }
}