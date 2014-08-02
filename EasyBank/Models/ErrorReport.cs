using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EasyBank.Models
{
    public class ErrorReport
    {
        public int ErrorReportId { get; set; }
        public string Text { get; set; }
        public bool Solved { get; set; }
        public DateTime ReportDate { get; set; }

        [ForeignKey("Account")]
        public int AccountId { get; set; }

        public Account Account { get; set; }
    }
}