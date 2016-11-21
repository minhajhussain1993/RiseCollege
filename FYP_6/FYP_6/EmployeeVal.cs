using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(EmployeeVal))]
    public partial class Employee
    { 
    }
    public class EmployeeVal
    {
        public System.Guid EmpID { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 30 characters")]
        public string UserName { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Password must be 5 to 30 characters")]
        public string Password { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Name must be Alphabetic")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters")]
        public string Name { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-Z0-9\d ,.]*$", ErrorMessage = "Address can be Alphanumeric with ,. Allowed only")]
        public string Address { get; set; }
        [RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$", ErrorMessage = "Plz Enter a Valid Phone No")]
        public string ContactNo { get; set; }
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Gender must be between 4 and 10 characters")]
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Martial status must be between 3 and 10 characters")]
        public string Martial_Status { get; set; }
        public byte[] Matric { get; set; }
        public byte[] Intermediate { get; set; }
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Salary must be an unsigned Number")]
        [Range(0,1000000)]
        public Nullable<decimal> Salary { get; set; }
        public string Status { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 15, ErrorMessage = " CNIC must be 15 characters")]
        [RegularExpression("^[0-9+]{5}-[0-9+]{7}-[0-9]{1}$", ErrorMessage = "Plz Enter Correct CNIC!")]
        public string CNIC { get; set; }
        public byte[] Picture { get; set; }

        public string Religion { get; set; }
    }
}