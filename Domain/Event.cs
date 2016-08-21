using System;
using System.Collections.Generic;

namespace Domain
{
    public class Event
    {
        public Event()
        {
            Expenses = new HashSet<Expense>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public bool Active { get; set; }

        public ICollection<Expense> Expenses { get; set; }
    }
}
