using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using RefactorThis.GraphDiff;

namespace Repository
{
    public class EFRepository : IFamilyRepository, IEventRepository
    {
        static EFRepository()
        {
            var type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
            if(type == null)
                throw new Exception("Do not remove, ensures static reference to System.Data.Entity.SqlServer");
        }

        public IEnumerable<Family> Get()
        {
            return Do<IEnumerable<Family>>(context => context.Families.ToList());
        }

        public Family CreateOrUpdate(Family family)
        {
            return Do(context => context.UpdateGraph(family));
        }

        void IFamilyRepository.DeleteAll()
        {
            Do(context => context.Database.ExecuteSqlCommand("delete from dbo.Family"));
        }

        public IEnumerable<Event> Get(bool? active = null)
        {
            return Do<IEnumerable<Event>>(context => context.Events.ToList());
        }

        public Event Get(int eventId)
        {
            return Do(context => context.Events
                                            .Include(e => e.Expenses
                                                            .Select(ex => ex.Attendances
                                                                            .Select(a => a.Family)))
                                         .SingleOrDefault(ev => ev.Id == eventId));
        }

        public Event CreateOrUpdate(Event thEvent)
        {
            thEvent.LastModifiedDate = DateTime.Now;
            return Do(context => 
                        context.UpdateGraph(thEvent, 
                                            mapping => mapping.OwnedCollection(
                                                                e => e.Expenses, 
                                                                mapping2 => mapping2.OwnedCollection(ex => ex.Attendances, 
                                                                                                     mapping3 => mapping3.AssociatedEntity(a => a.Family)))));
        }

        public void Delete(int eventId)
        {
            Do(context => context.Events.Remove(context.Events.Find(eventId)));
        }

        private static T Do<T>(Func<GoDutch, T> run)
        {
            using (var context = CreateContext())
            {
                var result = run(context);
                context.SaveChanges();
                return result;
            }
        }

        private static void Do(Action<GoDutch> run)
        {
            Do<object>((context) =>
            {
                run(context);
                return null;
            });
        }

        private static GoDutch CreateContext()
        {
            var context = new GoDutch();
            context.Database.Log = Console.WriteLine;

            return context;
        }
    }
}
