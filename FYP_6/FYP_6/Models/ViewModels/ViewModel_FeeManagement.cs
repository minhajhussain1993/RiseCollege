using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_6.Models.ViewModels
{
    public class ViewModel_FeeManagement
    {
        public Fee yearlyFee { get; set; }
        public Overall_Fees feeSummary { get; set; }

        public string Name { get; set; }
    }
}