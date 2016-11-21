using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.ViewModels
{
    public class ViewModel_TeacherBatchSubjects
    {
        public List<Subject> SubjBatch { get; set; }
        public List<Teacher_Subject> TeacherSubjBatch { get; set; }
    }
}