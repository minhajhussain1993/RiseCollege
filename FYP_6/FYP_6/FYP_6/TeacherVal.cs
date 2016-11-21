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
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Name must be between 15 and 5 characters")]
        [Required]
        public string Name { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be between 15 and 5 characters")]
        public string Address { get; set; }
        [RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$", ErrorMessage = "Plz Enter a Valid Phone No")]
        public string ContactNo { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Gender must be between 10 and 4 characters")]
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Martial Status must be between 15 and 3 characters")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Martial_Status must be Alphabetic")]
        public string Martial_Status { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Graduation_Details must be between 15 and 3 characters")]
        public string Graduation_Details { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Post_Graduation_Details must be between 15 and 3 characters")]
        public string Post_Graduation_Details { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Year_of_Graduation must be a Number")]
        [Required]
        public Nullable<int> Year_of_Graduation { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Year_of_Graduation must be a Number")]
        public Nullable<int> Year_of_Post_Graduation { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Password must be 5 to 30 characters")]
        public string Password { get; set; }
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$", ErrorMessage = "Salary must be a Number")]
        public Nullable<decimal> Salary { get; set; }
        public string Status { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Major_Subject must be between 30 and 3 characters")]
        public string Major_Subject { get; set; }
        public byte[] Picture { get; set; }

    
        
        public virtual ICollection<Teachers_Batches> Teachers_Batches { get; set; }
        public virtual ICollection<Teacher_Attendance> Teacher_Attendance { get; set; }
    }
}