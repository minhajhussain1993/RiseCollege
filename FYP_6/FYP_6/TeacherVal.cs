using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP_6
{
    [MetadataType(typeof(TeacherVal))]
    public partial class Teacher
    {

    }
    public class TeacherVal
    {
        public TeacherVal()
        {
            this.Teachers_Batches = new HashSet<Teachers_Batches>();
            this.Teacher_Attendance = new HashSet<Teacher_Attendance>();
        }
        [Required]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "TeacherID must be between 3 and 25 characters")]
        [Remote("ValidateTeacherID", "Home", ErrorMessage = "This ID is Already Assigned")]
        public string TeacherID { get; set; }
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Name must be Alphabetic")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 30 characters")]
        [Required]
        public string Name { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 30 characters")]
        [RegularExpression(@"[a-zA-Z0-9\d ,.]*$", ErrorMessage = "Address can be Alphanumeric with ,. Allowed only")]
        public string Address { get; set; }
        [RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$", ErrorMessage = "Plz Enter a Valid Phone No")]
        public string ContactNo { get; set; }
        //[Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Gender must be between 10 and 4 characters")]
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Martial Status must be between 3 and 15 characters")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Martial_Status must be Alphabetic")]
        public string Martial_Status { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Graduation Level must be between 1 and 15 characters")]
        public string Graduation_Degree_Level { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Post_Graduation Level must be between 1 and 15 characters")]
        public string Post_Graduation_Level { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Year_of_Graduation must be a Number")]
        [Required]
        [Range(1700, 9999, ErrorMessage = "Plz Provide Valid Year")]
        public Nullable<int> Year_of_Graduation { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Year_of_Graduation must be a Number")]
        [Range(1700, 9999, ErrorMessage = "Plz Provide Valid Year")]
        public Nullable<int> Year_of_Post_Graduation { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Password must be 5 to 30 characters")]
        public string Password { get; set; }
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Salary must be an unsigned Number")]
        [Range(0,1000000)]
        public Nullable<decimal> Salary { get; set; }
        public string Status { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Degree Name must be Alphabetic")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Degree Name must be between 3 and 50 characters")]

        public string Graduation_Degree_Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Institution  Name must be Alphabetic")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Institution  Name must be between 3 and 100 characters")]
        public string Graduation_Degree_Institution { get; set; }
        //[Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Degree Name must be Alphabetic")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Degree Name must be between 3 and 50 characters")]
        public string Post_Graduation_Degree_Name { get; set; }
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Institution  Name must be Alphabetic")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Institution Name must be between 3 and 100 characters")]
        public string Post_Graduation_Degree_Institution { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 15, ErrorMessage = " CNIC must be 15 characters")]
        [RegularExpression("^[0-9+]{5}-[0-9+]{7}-[0-9]{1}$", ErrorMessage = "Plz Enter Correct CNIC!")]
        public string CNIC { get; set; }
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Religion Field must be 3 to 20 characters")]
        public string Religion { get; set; }
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Major_Subject must be Alphabetic")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Major_Subject must be between 3 and 30 characters")]
        public string Major_Subject { get; set; }
        public byte[] Picture { get; set; }

    
        
        public virtual ICollection<Teachers_Batches> Teachers_Batches { get; set; }
        public virtual ICollection<Teacher_Attendance> Teacher_Attendance { get; set; }
    }
}