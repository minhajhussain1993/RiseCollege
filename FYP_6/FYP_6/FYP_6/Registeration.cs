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
    
    public partial class Registeration
    {
        public Registeration()
        {
            this.Assign_Subject = new HashSet<Assign_Subject>();
            this.Fees = new HashSet<Fee>();
        }
    
        public string Rollno { get; set; }
        public string RegisterationNo { get; set; }
        public string BatchID { get; set; }
        public Nullable<System.DateTime> DateOfRegisteration { get; set; }
        public string Password { get; set; }
        public Nullable<System.Guid> ProfileID { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> Part { get; set; }
    
        public virtual ICollection<Assign_Subject> Assign_Subject { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual Part Part1 { get; set; }
        public virtual Student_Profile Student_Profile { get; set; }
    }
}
