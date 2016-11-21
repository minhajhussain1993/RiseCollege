using FYP_6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP_6
{
    [MetadataType(typeof(RegisterationVal))]
    public partial class Registeration
    {

    }
    public class RegisterationVal
    {
        public RegisterationVal()
        {
            this.Assign_Subject = new HashSet<Assign_Subject>();
            this.Fees = new HashSet<Fee>();
        }
        [Remote("ValidateRollno", "Home", ErrorMessage = "Roll Already Exists")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Roll no is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Roll no must be 3 to 15 characters")]
        public string Rollno { get; set; }
        public string RegisterationNo { get; set; }
        public string BatchID { get; set; }
        public Nullable<int> Part { get; set; }
        public Nullable<System.DateTime> DateOfRegisteration { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Password must be 5 to 30 characters")]
        public string Password { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.Guid> ProfileID { get; set; }
    
        public virtual ICollection<Assign_Subject> Assign_Subject { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual Part Part1 { get; set; }
        
        public virtual Student_Profile Student_Profile { get; set; }
    }
}