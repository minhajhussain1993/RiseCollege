﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.Report_Models
{
    public class ReportClassForStudentsListByEmployee
    {
        public string Rollno { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Degree_ProgramName { get; set; }
        public int Part { get; set; }
        public string Duration { get; set; }
        public string Section { get; set; }
    }
}