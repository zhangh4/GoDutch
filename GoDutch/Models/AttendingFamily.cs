using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoDutch.Models
{
    public class AttendingFamily : Family
    {
        public double Expense { get; set; }
        public double Count { get; set; }
    }
}