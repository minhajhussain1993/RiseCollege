//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYP_6
{
    using System;
    using System.Collections.Generic;
    
    public partial class Assign_Subject
    {
        public Assign_Subject()
        {
            this.Student_Marks = new HashSet<Student_Marks>();
            this.Students_Attendance = new HashSet<Students_Attendance>();
        }
    
        public System.Guid AssignID { get; set; }
        public string Rollno { get; set; }
        public Nullable<System.Guid> Batch_Subject_ID { get; set; }
        public string Status { get; set; }
    
        public virtual Batch_Subjects_Parts Batch_Subjects_Parts { get; set; }
        public virtual Registeration Registeration { get; set; }
        public virtual ICollection<Student_Marks> Student_Marks { get; set; }
        public virtual ICollection<Students_Attendance> Students_Attendance { get; set; }
    }
}
