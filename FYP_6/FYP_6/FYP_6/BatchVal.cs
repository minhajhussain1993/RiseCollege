using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(BatchVal))]
    public partial class Batch
    {

    }
    public class BatchVal
    {
        public BatchVal()
        {
            this.Batch_Subjects_Parts = new HashSet<Batch_Subjects_Parts>();
            this.Registerations = new HashSet<Registeration>();
            this.Teachers_Batches = new HashSet<Teachers_Batches>();
        }
    
        public System.Guid BatchID { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Batch Name must be between 1 and 15 characters")]
        public string BatchName { get; set; }
        public Nullable<System.Guid> YearID { get; set; }
        public Nullable<System.Guid> SectionID { get; set; }
        public Nullable<System.Guid> DegreeProgram_ID { get; set; }
        public Nullable<int> Status { get; set; }
    
        public virtual Degree_Program Degree_Program { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Batch_Subjects_Parts> Batch_Subjects_Parts { get; set; }
        public virtual Year Year { get; set; }
        public virtual ICollection<Registeration> Registerations { get; set; }
        public virtual ICollection<Teachers_Batches> Teachers_Batches { get; set; }
    }
}