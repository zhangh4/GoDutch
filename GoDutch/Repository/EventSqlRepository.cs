using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using GoDutch.Models;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;

namespace GoDutch.Repository
{
    public class EventSqlRepository : SqlRepositoryBase, IEventRepository
    {
        public EventSqlRepository() : this(ConfigurationManager.ConnectionStrings["GoDutch"].ToString())
        {
        }

        private EventSqlRepository(string connectionString) : base(connectionString)
        {
        }

        [Dependency]
        public IFamilyRepository FamilyRepository { get; set; }

        public IEnumerable<Event> Get()
        {
            const string eventSql = @"select Id, Name from dbo.Event order by LastModifiedDate desc";
            
            var eventId2AttendingFamilies = GetAllAttendingFamilies().GroupBy(f => f.EventId).ToDictionary(o => o.Key, o => o);

            using (var reader = Sql.ExecuteReader(eventSql))
            {
                while (reader.Read())
                {
                    var eventId = reader.Get<int>("Id");
                    yield return new Event()
                    {
                        Id = eventId,
                        Name = reader.Get<string>("Name"),
                        AttendingFamilies = eventId2AttendingFamilies.GetOrNull(eventId)
                    };
                }
            }
        }

        public Event Create(Event newEvent)
        {
            if (newEvent == null) throw new ArgumentNullException("newEvent");
            if (string.IsNullOrWhiteSpace(newEvent.Name)) throw new ArgumentException("Name in newEvent is null or empty");

            const string insertEventSql = @"insert into dbo.Event(Name, LastModifiedDate) values (@Name, @LastModifiedDate); 
                                            SELECT CAST(scope_identity() AS int);";

            using (var ts = new TransactionScope())
            {
                newEvent.Id = Sql.ExecuteScalar<int>(
                                    insertEventSql,
                                    new SqlParameter("@Name", newEvent.Name),
                                    new SqlParameter("@LastModifiedDate", DateTime.Now));

                CreateAttendingFamilies(newEvent.Id, newEvent.AttendingFamilies);

                ts.Complete();

                return newEvent;    
            }
        }

        public void Update(Event updatedEvent)
        {
            if (updatedEvent == null) throw new ArgumentNullException("updatedEvent");
            if (string.IsNullOrWhiteSpace(updatedEvent.Name)) throw new ArgumentException("Name in updatedEvent is null or empty");

            const string sql = @"update dbo.Event set Name = @Name, LastModifiedDate = @LastModifiedDate where Id = @Id";

            using (var ts = new TransactionScope())
            {
                Sql.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@Name", updatedEvent.Name),
                        new SqlParameter("@LastModifiedDate", DateTime.Now),
                        new SqlParameter("@Id", updatedEvent.Id));

                DeleteAttendingFamilies(updatedEvent.Id);

                CreateAttendingFamilies(updatedEvent.Id, updatedEvent.AttendingFamilies);    

                ts.Complete();
            }
        }

        public void Delete(int eventId)
        {
            const string sql = @"delete from dbo.Event where Id = @Id";

            Sql.ExecuteNonQuery(sql, new SqlParameter("@Id", eventId));
        }

        private IEnumerable<AttendingFamilyDto> GetAllAttendingFamilies()
        {
            const string attendingFamiliesSql = @"SELECT EventId, FamilyId, Expense, Count FROM dbo.AttendingFamily af";

            var familiyId2Name = FamilyRepository
                .Get()
                .ToDictionary(o => o.Id, o => o.Name);

            using (var reader = Sql.ExecuteReader(attendingFamiliesSql))
            {
                while (reader.Read())
                {
                    var familyId = reader.Get<int>("FamilyId");
                    yield return new AttendingFamilyDto()
                    {
                        Id = familyId,
                        Name = familiyId2Name.GetOrNull(familyId),
                        EventId = reader.Get<int>("EventId"),
                        Expense = reader.Get<decimal?>("Expense"),
                        Count = reader.Get<double?>("Count")
                    };    
                }
            }
        }

        private void CreateAttendingFamilies(int eventId, IEnumerable<AttendingFamily> attendingFamilies)
        {
            const string insertAttendingFamiliesSql =
                @"insert into dbo.AttendingFamily (EventId, FamilyId, Expense, Count) values (@EventId, @FamilyId, @Expense, @Count)";

            if (attendingFamilies != null)
            {
                foreach (var attendingFamily in attendingFamilies)
                {
                    Sql.ExecuteNonQuery(
                        insertAttendingFamiliesSql,
                        new SqlParameter("@EventId", eventId),
                        new SqlParameter("@FamilyId", attendingFamily.Id),
                        new SqlParameter("@Expense", attendingFamily.Expense),
                        new SqlParameter("@Count", attendingFamily.Count));
                }
            }
        }

        private void DeleteAttendingFamilies(int eventId)
        {
            const string sql = @"delete from dbo.AttendingFamily where EventId = @EventId";

            Sql.ExecuteNonQuery(sql, new SqlParameter("@EventId", eventId));
        }

        private class AttendingFamilyDto : AttendingFamily
        {
            public int EventId { get; set; }
        }
    }
}