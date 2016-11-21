using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.Report_Models
{
    public class RptFeeSummaryClass
    { 
        public string RollNo { get; set; }
        public decimal Total_Degree_Fee { get; set; }
        public decimal SubmittedFee { get; set; }
        public decimal RemainingFee { get; set; }
        public int Total_Installments { get; set; }
        public int Paid_Installments { get; set; }
    }
}