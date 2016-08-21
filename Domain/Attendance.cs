namespace Domain
{
    public class Attendance
    {
        public int Id { get; set; }

//        public int FamilyId { get; set; }

        // this property is for EF to cascade on delete
        public int ExpenseId { get; set; }

//        [Column(TypeName = "money")]
        public decimal? Cost { get; set; }

        public double? HeadCount { get; set; }

//        public Expense Expense { get; set; }

        public Family Family { get; set; }
    }
}
