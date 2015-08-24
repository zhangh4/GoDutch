using System;
using System.Collections.Generic;
using System.Linq;

namespace GoDutch.Common.Models
{
    public class Expense
    {
        private IEnumerable<AttendingFamily> attendingFamilies;

        public string Name { get; set; }

        public IEnumerable<AttendingFamily> AttendingFamilies
        {
            get { return attendingFamilies ?? Enumerable.Empty<AttendingFamily>(); }
            set { attendingFamilies = value; }
        } 

        protected bool Equals(Expense other)
        {
            return string.Equals(Name, other.Name) && new HashSet<AttendingFamily>(AttendingFamilies).SetEquals(other.AttendingFamilies);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Expense) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
//                hashCode = (hashCode*397) ^ (AttendingFamilies != null ? AttendingFamilies.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, AttendingFamilies: {1}", 
                Name, AttendingFamilies == null ? "": AttendingFamilies.Aggregate(string.Empty, (family, result) => string.Format("{0}, [{1}]", result, family.ToString())));
        }
    }
}