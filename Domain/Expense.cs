using System.Collections.Generic;

namespace Domain
{
    public class Expense
    {
        public Expense()
        {
            Attendances = new HashSet<Attendance>();
        }

        public int Id { get; set; }

        // this property is for EF to cascade on delete
        public int EventId { get; set; }

        public string Name { get; set; }

        public ICollection<Attendance> Attendances { get; set; }

//        public Event Event { get; set; }
    }
}
