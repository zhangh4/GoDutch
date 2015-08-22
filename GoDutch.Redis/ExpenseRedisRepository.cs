using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using StackExchange.Redis;

namespace GoDutch.Redis
{
    public class ExpenseRedisRepository : RedisRepositoryBase, IExpenseRepository
    {
        private const string Prefix_Expense = "expense:";
        private const string Prefix_Expenses_Event = "expenses.event:";

        public ExpenseRedisRepository(IConnectionMultiplexer connection, string host, int port) : base(connection, host, port)
        {
        }

        public Expense Create(Expense newExpense)
        {
            var db = connection.GetDatabase();
            newExpense.Id = (int)db.StringIncrement("id.expense");
            var result = PersistExpense(newExpense);
            db.SetAdd(FormatExpensesEventKey(newExpense.EventId), newExpense.Id);
            return result;
        }

        public void Update(Expense updatedExpense)
        {
            PersistExpense(updatedExpense);
        }

        public void Delete(int expenseId)
        {
            var db = connection.GetDatabase();
            var expenseKey = FormatExpenseKey(expenseId);
            var eventId = (int)db.HashGet(expenseKey, "event");
            db.SetRemove(FormatExpensesEventKey(eventId), expenseId);
            db.KeyDelete(expenseKey);
            // todo: delete attending families
        }

        public IEnumerable<Expense> Get(int eventId)
        {
            var db = connection.GetDatabase();
            
            var expenseIds = db.SetMembers(FormatExpensesEventKey(eventId));
            foreach (var expenseId in expenseIds)
            {
                var values = db.HashGet(FormatExpenseKey((int)expenseId), new RedisValue[] {"name", "createdDate", "event"});
                yield return 
                    new Expense()
                    {
                        Id = (int) expenseId,
                        Name = values[0],
                        CreateDateTime = DateTime.Parse(values[1]),
                        EventId = (int) values[2]
                    };
            }
                
        }

        private static string FormatExpenseKey(int id)
        {
            return String.Format("{0}{1}", Prefix_Expense, id);
        }

        private static string FormatExpensesEventKey(int id)
        {
            return String.Format("{0}{1}", Prefix_Expenses_Event, id);
        }

        private Expense PersistExpense(Expense expense)
        {
            expense.CreateDateTime = DateTime.Now;
            var db = connection.GetDatabase();
            db.HashSet(FormatExpenseKey(expense.Id),
                        new[]{ new HashEntry("name", expense.Name), 
                                new HashEntry("createdDate", expense.CreateDateTime.ToString("O")),
                                new HashEntry("event", expense.EventId) });
            return expense;
        }
    }
}
