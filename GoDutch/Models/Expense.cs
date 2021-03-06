﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoDutch.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<AttendingFamily> AttendingFamilies { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int EventId { get; set; }
    }
}