using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class ErrorReport
    {
        public int ErrorReportId { get; set; }
        public string Text { get; set; }
        public string AccountNumber { get; set; }
        public bool Solved { get; set; }
        public DateTime ReportDate { get; set; }
    }
}