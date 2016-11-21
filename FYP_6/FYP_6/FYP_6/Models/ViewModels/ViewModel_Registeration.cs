using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.ViewModels
{
    public class ViewModel_Registeration
    {
        public Student_Profile stdProfile { get; set; }
        public Registeration stdRegisteration { get; set; }

        public IEnumerable<Batch_Subjects_Parts> BSP { get; set; }
    }
}