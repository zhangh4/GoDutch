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
        public DateTime CreateDateTime { get; set; }

        protected bool Equals(Event other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Event) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}