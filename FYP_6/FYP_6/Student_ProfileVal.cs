using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(Student_ProfileVal))]
    public partial class Student_Profile
    {

    }
    public class Student_ProfileVal
    {
        public Student_ProfileVal()
        {
            this.Registerations = new HashSet<Registeration>();
        }

        public System.Guid ProfileID { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "FirstName must be 3 to 15 characters")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "First Name must be Alphabetic")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = " LastName must be 3 to 15 characters")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "LastName must be Alphabetic")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        //[Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Gender must be 4 to 10 characters")]
        public string Gender { get; set; }
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Religion Field must be 3 to 15 characters")]
        public string Religion { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 15, ErrorMessage = " CNIC must be 15 characters")]
        [RegularExpression("^[0-9+]{5}-[0-9+]{7}-[0-9]{1}$",ErrorMessage="Plz Enter Correct CNIC!")]
        public string CNIC { get; set; }
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Address must be 3 to 30 characters")]
        [RegularExpression(@"[a-zA-Z0-9\d ,.]*$", ErrorMessage = "Address can be Alphanumeric with ,. Allowed only")]
        public string Address { get; set; }

[RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$", ErrorMessage = "Plz Enter a Valid Phone No")]

        public string ContactNo { get; set; }
        [Required(ErrorMessage = "Total Matric Marks are Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total Matric_Marks must be a Number")]
        [Range(1, 1100)]
        public Nullable<int> Total_Matric_Marks { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total Intermediate_Marks must be a Number")]
        [Range(1,1100)]
        public Nullable<int> Total_Inter_Marks { get; set; }
        [Required(ErrorMessage = "Obtained Matric Marks are required")]
        [Range(0, 1100)]
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Obtained Matric Marks must be an unsigned Number")]
        public Nullable<double> Matric_Marks { get; set; }
        [Range(0, 1100)]
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Obtained Inter Marks must be an unsigned Number")]
        public Nullable<double> Intermediate_Marks { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Domicile must be 3 to 15 characters")]

        public string Domicile { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Province must be 1 to 30 characters")]
        public string Province { get; set; }
        public byte[] Picture { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Father/Guardian_Name must be Alphabetic")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = " Father/Guardian_Name must be 3 to 30 characters")]

        public string FatherName_Guardian_Name { get; set; }

        [RegularExpression("^((\\+92)|(0092))-{0,1}\\d{3}-{0,1}\\d{7}$|^\\d{11}$|^\\d{4}-\\d{7}$", ErrorMessage = "Plz Enter a Valid Phone No")]
        public string Father_Guardian_Contact { get; set; }
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Father/Guardian Occupation must be Alphabetic")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = " Father/Guardian Occupation must be 3 to 30 characters")]
        public string Father_Guardian_Occupation { get; set; }

        [Required(ErrorMessage = "School Name is Required")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "School Name must be Alphabetic")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = " School Name must be 3 to 100 characters")]
        public string MatricFrom { get; set; }
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "College Name must be Alphabetic")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = " College Name must be 3 to 100 characters")]
        public string IntermediateFrom { get; set; }
        public string Status { get; set; }
    
        public virtual ICollection<Registeration> Registerations { get; set; }
    }
}