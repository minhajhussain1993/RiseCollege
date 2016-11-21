using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.Report_Models
{
    public class FeeReportCLass
    {
        public string Bill_No { get; set; }
        public string Rollno { get; set; }
        public string StudentName { get; set; }
        public  DateTime Dated { get; set; }
        public decimal Registeration_Fee { get; set; }
        public decimal Tution_Fee { get; set; }
        public decimal Admission_Fee { get; set; }
        public decimal Exam_Fee { get; set; }
        public decimal Total_Installments { get; set; }
        public int Installment { get; set; }
        public string Month { get; set; }
        public decimal Fine { get; set; }
        public decimal Total_Fee { get; set; }
    }
}