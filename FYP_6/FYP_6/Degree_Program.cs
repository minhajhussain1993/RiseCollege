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
    
    public partial class Degree_Program
    {
        public Degree_Program()
        {
            this.Batches = new HashSet<Batch>();
            this.Degree_Subject = new HashSet<Degree_Subject>();
        }
    
        public System.Guid ProgramID { get; set; }
        public string Degree_ProgramName { get; set; }
        public Nullable<System.Guid> LevelID { get; set; }
    
        public virtual ICollection<Batch> Batches { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<Degree_Subject> Degree_Subject { get; set; }
    }
}