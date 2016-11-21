using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6.Models.ViewModels
{
    public class ViewModelMarks
    {
        //Assign Subjects Model Class
        public System.Guid AssignID { get; set; }
        public string Rollno { get; set; }
        public Nullable<System.Guid> Batch_Subject_ID { get; set; }
        public virtual Batch_Subjects_Parts Batch_Subjects_Parts { get; set; }
        public virtual Registeration Registeration { get; set; }
        public virtual ICollection<Students_Attendance> Students_Attendance { get; set; }
        public virtual ICollection<Student_Marks> Student_Marks { get; set; }


        //Marks Model Class
        public System.Guid ResultID { get; set; }
        public Nullable<System.Guid> SubjectAssignID { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Total Marks must be a Number")]

        public Nullable<int> Total_Marks { get; set; }
        [Required]
        [RegularExpression(@"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$",
            ErrorMessage = "Obtained_Marks must be a Number")]

        public Nullable<double> Obtained_Marks { get; set; }

        public string Marks_Percentage { get; set; }

        public string Month { get; set; }
        public Nullable<int> Year { get; set; }

        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}