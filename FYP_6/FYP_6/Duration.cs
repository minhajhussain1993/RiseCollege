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
    
    public partial class Duration
    {
        public Duration()
        {
            this.Levels = new HashSet<Level>();
        }
    
        public System.Guid Id { get; set; }
        public string Duration1 { get; set; }
    
        public virtual ICollection<Level> Levels { get; set; }
    }
}
