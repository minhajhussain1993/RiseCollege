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
    
    public partial class Teachers_Batches
    {
        public Teachers_Batches()
        {
            this.Teacher_Subject = new HashSet<Teacher_Subject>();
        }
    
        public System.Guid ID { get; set; }
        public string TeacherID { get; set; }
        public string BatchName { get; set; }
    
        public virtual Batch Batch { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Teacher_Subject> Teacher_Subject { get; set; }
    }
}
