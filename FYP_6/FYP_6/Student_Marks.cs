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
    
    public partial class Student_Marks
    {
        public System.Guid ResultID { get; set; }
        public Nullable<System.Guid> SubjectAssignID { get; set; }
        public Nullable<int> Total_Marks { get; set; }
        public Nullable<double> Obtained_Marks { get; set; }
        public string Marks_Percentage { get; set; }
        public string Month { get; set; }
        public Nullable<int> Year { get; set; }
        public string Status { get; set; }
    
        public virtual Assign_Subject Assign_Subject { get; set; }
    }
}
