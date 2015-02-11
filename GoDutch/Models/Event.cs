using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoDutch.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<AttendingFamily> AttendingFamilies { get; set; }
    }
}