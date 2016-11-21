using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(StudentMarksVal))]
    public partial class Student_Marks
    {

    }
    public class StudentMarksVal
    {
        public System.Guid ResultID { get; set; }
        public Nullable<System.Guid> SubjectAssignID { get; set; }
        [Required(ErrorMessage = "Total Marks are required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total_Marks must be an unsigned Number")]
        [Range(1, 100)]
        public Nullable<int> Total_Marks { get; set; }
        [Required]
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Obtained Marks must be an unsigned Number")]
        public Nullable<double> Obtained_Marks { get; set; }
        [RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Percentage must be an unsigned Number")]
        public string Marks_Percentage { get; set; }
        public string Month { get; set; }
        public Nullable<int> Year { get; set; }
        public string Status { get; set; }

        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}