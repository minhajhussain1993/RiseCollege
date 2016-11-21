using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6.Models.ViewModels
{
    public class ViewModelAttendance
    {
        //Assign Subjects Model Class
        public System.Guid AssignID { get; set; }
        public string Rollno { get; set; }
        public Nullable<System.Guid> Batch_Subject_ID { get; set; }
        public virtual Batch_Subjects_Parts Batch_Subjects_Parts { get; set; }
        public virtual Registeration Registeration { get; set; }
        public virtual ICollection<Students_Attendance> Students_Attendance { get; set; }
        public virtual ICollection<Student_Marks> Student_Marks { get; set; }

        //Attendance Model Class
        public System.Guid AttendanceID { get; set; }
        public Nullable<System.Guid> SubjectAssignID { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total Lectures must be a Number")]

        public Nullable<int> Total_lectures { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Attended Lectures must be a Number")]
        public Nullable<int> Attended_Lectures { get; set; }
        public string Attendance_Percentage { get; set; }

        public string Month { get; set; }
        public Nullable<int> Year { get; set; }

        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}