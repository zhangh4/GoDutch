using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using Microsoft.Practices.Unity;
using ServiceStack.Redis;

namespace GoDutch.Redis
{
    public class EventRedisRepository : RedisRepoBase, IEventRepository
    {
        public IEnumerable<Event> Get(bool? active = null)
        {
            using (var client = Manager.GetClient())
            {
                var eventClient = client.As<Event>();
                return eventClient.GetAll();
            }
        }

        public Event Get(int eventId)
        {
            using (var client = Manager.GetClient())
            {
                var eventClient = client.As<Event>();
                return eventClient.GetById(eventId);
            }
        }

        public Event Create(string eventName)
        {
            return CreateOrUpdate(new Event() {Name = eventName});
        }

        public Event CreateOrUpdate(Event thEvent)
        {
            using (var client = Manager.GetClient())
            {
                var eventClient = client.As<Event>();
                thEvent.CreateDateTime = DateTime.Now;
                if (thEvent.Id == 0) thEvent.Id = (int) eventClient.GetNextSequence();
                return client.Store(thEvent);
            }
        }

        public void Delete(int eventId)
        {
            using (var client = Manager.GetClient())
            {
                var eventClient = client.As<Event>();
                eventClient.DeleteById(eventId);
            }
        }

    }
}
