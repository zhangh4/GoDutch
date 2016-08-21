using System.Data.Entity.ModelConfiguration.Conventions;
using Domain;
using NUnit.Framework.Internal;

namespace Repository
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GoDutch : DbContext
    {
        public GoDutch()
            : base("name=GoDutch")
        {
//            Database.SetInitializer(new DropCreateDatabaseAlways<GoDutch>());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GoDutch>());

            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<Family> Families { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Attendance>()
                .Property(e => e.Cost)
                .HasPrecision(19, 2);

//            modelBuilder.Entity<Family>()
//                .HasMany(e => e.Attendances)
//                .WithRequired(e => e.Family)
//                .WillCascadeOnDelete(false);
        }
    }
}
