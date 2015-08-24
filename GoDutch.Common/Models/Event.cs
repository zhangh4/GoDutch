using System;
using System.Collections.Generic;
using System.Linq;

namespace GoDutch.Common.Models
{
    public class Event
    {
        private IEnumerable<Expense> expenses; 

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDateTime { get; set; }

        public IEnumerable<Expense> Expenses
        {
            get
            {   
                return expenses ?? Enumerable.Empty<Expense>();
            }
            set { expenses = value; }
        }

        protected bool Equals(Event other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && CreateDateTime.ToString("G").Equals(other.CreateDateTime.ToString("G")) 
                && new HashSet<Expense>(Expenses).SetEquals(other.Expenses);
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
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CreateDateTime.GetHashCode();
                if (Expenses != null)
                {
                    Expenses.Aggregate(hashCode,
                        (i, expense) => (i*397) ^ expense.GetHashCode());
                }
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, CreateDateTime: {2}, Expenses: {3}, ", Id, Name, CreateDateTime.ToString("G"),
                Expenses.Aggregate(string.Empty, (expense, result) => string.Format("{0}, [{1}]", result, expense.ToString())));
        }
    }
}