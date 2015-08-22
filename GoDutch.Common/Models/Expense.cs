using System;
using System.Collections.Generic;
using System.Linq;

namespace GoDutch.Common.Models
{
    public class Expense
    {
        private IEnumerable<AttendingFamily> attendingFamilies;

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int EventId { get; set; }

        public IEnumerable<AttendingFamily> AttendingFamilies
        {
            get { return attendingFamilies ?? Enumerable.Empty<AttendingFamily>(); }
            set { attendingFamilies = value; }
        } 

        protected bool Equals(Expense other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && CreateDateTime.Equals(other.CreateDateTime) && EventId == other.EventId
                && new HashSet<AttendingFamily>(AttendingFamilies).SetEquals(other.AttendingFamilies);
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
                var hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
//                hashCode = (hashCode*397) ^ (AttendingFamilies != null ? AttendingFamilies.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CreateDateTime.GetHashCode();
                hashCode = (hashCode*397) ^ EventId;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, CreateDateTime: {2}, EventId: {3}, AttendingFamilies: {4}", 
                Id, Name, CreateDateTime.ToString("O"), EventId,
                AttendingFamilies == null ? "": AttendingFamilies.Aggregate(string.Empty, (family, result) => string.Format("{0}, [{1}]", result, family.ToString())));
        }
    }
}