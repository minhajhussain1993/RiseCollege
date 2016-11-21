using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.Report_Models
{
    public class FeeReportModel
    {
        public string Bill_No { get; set; }
        public string Rollno { get; set; }
        public string StudentName { get; set; }
        public Nullable<System.DateTime> Dated { get; set; }
        public Nullable<decimal> Registeration_Fee { get; set; }
        public Nullable<decimal> Tution_Fee { get; set; }
        public Nullable<decimal> Admission_Fee { get; set; }
        public Nullable<decimal> Exam_Fee { get; set; }
        public Nullable<int> Total_Installments { get; set; }
        public Nullable<int> Installment { get; set; }
        public string Month { get; set; }
        public Nullable<decimal> Fine { get; set; }
        public Nullable<decimal> Total_Fee { get; set; }
    }
}