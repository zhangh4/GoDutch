using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using Microsoft.Practices.Unity;
using StackExchange.Redis;

namespace GoDutch.Redis
{
    public class EventRedisRepository : RedisRepositoryBase, IEventRepository
    {
        private const string Prefix_Event = "event:";

        public EventRedisRepository(IConnectionMultiplexer connection, string host, int port) : base(connection, host, port)
        {
        }

        [Dependency]
        public IExpenseRepository ExpenseRepository { get; set; }

        public IEnumerable<Event> Get(bool? active = null)
        {
            throw new NotImplementedException();
        }

        public Event Get(int eventId)
        {
            var db = connection.GetDatabase();
            var values = db.HashGet(FormatKey(eventId), new RedisValue[]{"name", "createdDate"});
            if (values.All(o => o.IsNull)) return null;
            return new Event()
            {
                Id = eventId, 
                Name = values[0], 
                CreateDateTime = DateTime.Parse(values[1]), 
                Expenses = new HashSet<Expense>(ExpenseRepository.Get(eventId))
            };
        }

        public Event Create(string eventName)
        {
            var db = connection.GetDatabase();
            var id = (int)db.StringIncrement("id.event");
            var now = DateTime.Now;
            db.HashSet(FormatKey(id), 
                        new []{ new HashEntry("name", eventName), 
                                new HashEntry("createdDate", now.ToString("O")) });
            return new Event() { Id = id, Name = eventName, CreateDateTime = now, Expenses = Enumerable.Empty<Expense>()};
        }

        public void Delete(int eventId)
        {
            var expenses = ExpenseRepository.Get(eventId);
            foreach (var expense in expenses)
            {
                ExpenseRepository.Delete(expense.Id);
            }
            var db = connection.GetDatabase();
            db.KeyDelete(FormatKey(eventId));
        }

        private static string FormatKey(int id)
        {
            return string.Format("{0}{1}", Prefix_Event, id);
        }
    }
}
