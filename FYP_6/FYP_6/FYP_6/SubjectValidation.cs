using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_6
{
    [MetadataType(typeof(SubjectValidation))]
    public partial class Subject
    {

    }
    public class SubjectValidation
    {
        public SubjectValidation()
        {
            this.Batch_Subjects_Parts = new HashSet<Batch_Subjects_Parts>();
            this.Degree_Subject = new HashSet<Degree_Subject>();
        }

        public System.Guid SubjectID { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "SubjectName must be Alphabetic")]

        public string SubjectName { get; set; }
    
        public virtual ICollection<Batch_Subjects_Parts> Batch_Subjects_Parts { get; set; }
        public virtual ICollection<Degree_Subject> Degree_Subject { get; set; }
    }
}