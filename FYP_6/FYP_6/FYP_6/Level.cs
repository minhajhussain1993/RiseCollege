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
    
    public partial class Level
    {
        public Level()
        {
            this.Degree_Program = new HashSet<Degree_Program>();
        }
    
        public System.Guid LevelID { get; set; }
        public string Level_Name { get; set; }
        public Nullable<System.Guid> DurationID { get; set; }
    
        public virtual ICollection<Degree_Program> Degree_Program { get; set; }
        public virtual Duration Duration { get; set; }
    }
}
