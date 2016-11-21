using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(StudentAttendanceVal))]
    public partial class Students_Attendance
    {

    }
    public class StudentAttendanceVal
    {
        public System.Guid AttendanceID { get; set; }
        public Nullable<System.Guid> SubjectAssignID { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total_lectures must be an unsigned Number")]
        [Range(1, 60)]
        public Nullable<int> Total_lectures { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Attended_Lectures must be an unsigned Whole Number")]
        public Nullable<int> Attended_Lectures { get; set; }
        //[RegularExpression(@"^(?:[1-9]\d*|0)?(?:\.\d+)?$", ErrorMessage = "Percentage must be an unsigned Number")]
        [StringLength(200,MinimumLength=1,ErrorMessage="Attendance Percentage must be 1 to 200")]
        public string Attendance_Percentage { get; set; }
        public string Month { get; set; }
        public Nullable<int> Year { get; set; }
        public string Status { get; set; }

        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}