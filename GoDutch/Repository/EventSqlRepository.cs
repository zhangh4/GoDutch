using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using Microsoft.Practices.Unity;

namespace GoDutch.Repository
{
    public class EventSqlRepository : SqlRepositoryBase, IEventRepository
    {
        public EventSqlRepository() : this(ConfigurationManager.ConnectionStrings["GoDutch"].ToString())
        {
        }

        private EventSqlRepository(string connectionString)
            : base(connectionString)
        {
        }

        [Dependency]
        public IExpenseRepository ExpenseRepository { get; set; }

        public IEnumerable<Event> Get(bool? active = null)
        {
            const string eventSql = @"select Id, Name from dbo.Event @where_clause order by LastModifiedDate desc";

            string actualEventSql =
                eventSql.Replace("@where_clause",
                                active == null ? string.Empty :
                                (active.Value ? "where Active = 1" :
                                                "where Active = 0"));

            using (var reader = Sql.ExecuteReader(actualEventSql))
            {
                while (reader.Read())
                {
                    yield return new Event()
                    {
                        Id = reader.Get<int>("Id"),
                        Name = reader.Get<string>("Name"),
                    };
                }
            }
        }

        public Event Get(int eventId)
        {
            const string eventSql = @"select Id, Name from dbo.Event where Id = @Id";
            using (var reader = Sql.ExecuteReader(eventSql, new SqlParameter("@Id", eventId)))
            {
                if (reader.Read())
                {
                    return new Event()
                    {
                        Id = reader.Get<int>("Id"),
                        Name = reader.Get<string>("Name"),
                        Expenses = ExpenseRepository.Get(eventId)
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public Event Create(string eventName)
        {
            if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("eventName is null or empty");

            const string insertExpenseSql = @"insert into dbo.Event(Name, LastModifiedDate) values (@Name, @LastModifiedDate); 
                                            SELECT CAST(scope_identity() AS int);";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    var id = Sql.ExecuteScalar<int>(
                                        insertExpenseSql,
                                        conn,
                                        transaction,
                                        new SqlParameter("@Name", eventName),
                                        new SqlParameter("@LastModifiedDate", DateTime.Now));

                    transaction.Commit();

                    return new Event() { Id = id, Name = eventName };
                }
            }
        }

        public void Delete(int eventId)
        {
            const string sql = @"delete from dbo.Event where Id = @Id";

            Sql.ExecuteNonQuery(sql, null, null, new SqlParameter("@Id", eventId));
        }
    }
}