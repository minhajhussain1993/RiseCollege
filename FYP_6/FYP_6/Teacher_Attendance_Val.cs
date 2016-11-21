using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(Teacher_Attendance_Val))]
    public partial class Teacher_Attendance
    {

    }
    public class Teacher_Attendance_Val
    {
        public System.Guid ID { get; set; }
        public string TeacherID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime Date { get; set; }
        public string Present { get; set; }

        public virtual Teacher Teacher { get; set; }
    }
}