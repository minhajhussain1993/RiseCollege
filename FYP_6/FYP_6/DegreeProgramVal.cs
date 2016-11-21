using FYP_6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(DegreeProgramVal))]
    public partial class Degree_Program
    { 
    }
    public class DegreeProgramVal
    {
        public DegreeProgramVal()
        {
            this.Batches = new HashSet<Batch>();
            this.Degree_Subject = new HashSet<Degree_Subject>();
        }

        public System.Guid ProgramID { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "Degree Name must be Alphabetic")]
        public string Degree_ProgramName { get; set; }

        public Nullable<System.Guid> LevelID { get; set; }

        public virtual ICollection<Batch> Batches { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<Degree_Subject> Degree_Subject { get; set; }
    }
}