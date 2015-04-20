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
    public class ExpenseSqlRepository : SqlRepositoryBase, IExpenseRepository
    {
        private Dictionary<int, string> familiyId2Name;

        public ExpenseSqlRepository() : this(ConfigurationManager.ConnectionStrings["GoDutch"].ToString())
        {
        }

        private ExpenseSqlRepository(string connectionString) : base(connectionString)
        {
            
        }

        [Dependency]
        public IFamilyRepository FamilyRepository { get; set; }

        public IEnumerable<Expense> Get(int eventId)
        {
            const string eventSql = @"select Id, Name from dbo.Expense where EventId = @EventId";

            var eventId2AttendingFamilies = GetAllAttendingFamilies(eventId).GroupBy(f => f.ExpenseId).ToDictionary(o => o.Key, o => o);

            using (var reader = Sql.ExecuteReader(eventSql, new SqlParameter("@EventId", eventId)))
            {
                while (reader.Read())
                {
                    var expenseId = reader.Get<int>("Id");
                    yield return new Expense()
                    {
                        Id = expenseId,
                        Name = reader.Get<string>("Name"),
                        EventId = eventId,
                        AttendingFamilies = eventId2AttendingFamilies.GetOrNull(expenseId)
                    };
                }
            }
        }

        public Expense Create(Expense newExpense)
        {
            if (newExpense == null) throw new ArgumentNullException("newExpense");
            if (string.IsNullOrWhiteSpace(newExpense.Name)) throw new ArgumentException("Name in newExpense is null or empty");
            if(newExpense.EventId <= 0) throw new ArgumentException("EventId in newExpense is not positive");

            const string insertExpenseSql = @"insert into dbo.Expense(Name, EventId) values (@Name, @EventId); 
                                            SELECT CAST(scope_identity() AS int);";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    newExpense.Id = Sql.ExecuteScalar<int>(
                                        insertExpenseSql,
                                        conn,
                                        transaction,
                                        new SqlParameter("@Name", newExpense.Name),
                                        new SqlParameter("@EventId", newExpense.EventId));

                    CreateAttendingFamilies(newExpense.Id, newExpense.AttendingFamilies, conn, transaction);

                    transaction.Commit();

                    return newExpense;
                }
            }
        }

        public void Update(Expense updatedExpense)
        {
            if (updatedExpense == null) throw new ArgumentNullException("updatedExpense");
            if (string.IsNullOrWhiteSpace(updatedExpense.Name)) throw new ArgumentException("Name in updatedExpense is null or empty");

            const string sql = @"update dbo.Expense set Name = @Name where Id = @Id";
             
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    Sql.ExecuteNonQuery(
                            sql,
                            conn,
                            transaction,
                            new SqlParameter("@Name", updatedExpense.Name),
                            new SqlParameter("@Id", updatedExpense.Id));

                    DeleteAttendingFamilies(updatedExpense.Id, conn, transaction);

                    CreateAttendingFamilies(updatedExpense.Id, updatedExpense.AttendingFamilies, conn, transaction);

                    transaction.Commit();
                }
            }
        }

        public void Delete(int expenseId)
        {
            const string sql = @"delete from dbo.Expense where Id = @Id";

            Sql.ExecuteNonQuery(sql, null, null, new SqlParameter("@Id", expenseId));
        }

        private IEnumerable<AttendingFamilyDto> GetAllAttendingFamilies(int eventId)
        {
            const string attendingFamiliesSql = 
                @"SELECT ExpenseId, FamilyId, Expense, Count 
                    FROM dbo.AttendingFamily af
                    join dbo.Expense ex on af.ExpenseId = ex.Id
                  where ex.EventId = @EventId";

            // todo: move the following to constructor, but unity will not load dependency in time somehow
            if (familiyId2Name == null)
            {
                familiyId2Name = FamilyRepository
                                    .Get()
                                    .ToDictionary(o => o.Id, o => o.Name);
            }
            

            using (var reader = Sql.ExecuteReader(attendingFamiliesSql, new SqlParameter("@EventId", eventId)))
            {
                while (reader.Read())
                {
                    var familyId = reader.Get<int>("FamilyId");
                    yield return new AttendingFamilyDto()
                    {
                        Id = familyId,
                        Name = familiyId2Name.GetOrNull(familyId),
                        ExpenseId = reader.Get<int>("ExpenseId"),
                        Expense = reader.Get<decimal?>("Expense"),
                        Count = reader.Get<double?>("Count")
                    };    
                }
            }
        }

        // todo: use ThreadLocal to avoid passing SqlConnection and SqlTransaction
        private void CreateAttendingFamilies(int expenseId, IEnumerable<AttendingFamily> attendingFamilies, SqlConnection conn, SqlTransaction tran)
        {
            const string insertAttendingFamiliesSql =
                @"insert into dbo.AttendingFamily (ExpenseId, FamilyId, Expense, Count) values (@ExpenseId, @FamilyId, @Expense, @Count)";

            if (attendingFamilies != null)
            {
                foreach (var attendingFamily in attendingFamilies)
                {
                    Sql.ExecuteNonQuery(
                        insertAttendingFamiliesSql,
                        conn,
                        tran,
                        new SqlParameter("@ExpenseId", expenseId),
                        new SqlParameter("@FamilyId", attendingFamily.Id),
                        new SqlParameter("@Expense", attendingFamily.Expense),
                        new SqlParameter("@Count", attendingFamily.Count));
                }
            }
        }

        private void DeleteAttendingFamilies(int expenseId, SqlConnection conn, SqlTransaction tran)
        {
            const string sql = @"delete from dbo.AttendingFamily where ExpenseId = @ExpenseId";

            Sql.ExecuteNonQuery(sql, conn, tran, new SqlParameter("@ExpenseId", expenseId));
        }

        private class AttendingFamilyDto : AttendingFamily
        {
            public int ExpenseId { get; set; }
        }
    }
}