using System.Collections.Generic;

namespace Domain
{
    public class Family
    {
        public Family()
        {
//            Attendances = new HashSet<Attendance>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

//        public virtual ICollection<Attendance> Attendances { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}", Id, Name);
        }
    }
}
