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

        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}