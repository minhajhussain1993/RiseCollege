using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.Report_Models
{
    public class MarksBYTeacherReportClass
    {
        public string RollNo { get; set; }
        public string Name { get; set; }
        public string SubjectName { get; set; }
        public int TotalMarks { get; set; }
        public double ObtainedMarks { get; set; }
        public string Marks_Percentage { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string BatchName { get; set; }
        public string SectionName { get; set; }
        public string Degree_Programme
        {
            get;
            set;
        }
        public string Part { get; set; }
    }
}