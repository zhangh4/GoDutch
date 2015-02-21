using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoDutch.Models;
using GoDutch.Utils;

namespace GoDutch.Repository
{
    public class EventRepository : IEventRepository
    {
        private IFamilyRepository familyRepo;

        private readonly List<Event> events;

        public EventRepository(IFamilyRepository familyRepo)
        {
            this.familyRepo = familyRepo;

            events = new List<Event>()
            {
                new Event()
                {
                    Id = Utility.GetNextId(), 
                    Name = "Werewolf",
                    CreateDateTime = DateTime.Now,
                    AttendingFamilies = familyRepo.Get().Select(f => new AttendingFamily()
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Expense = 10,
                        Count = 3
                    })
                }
            };
        }

        public IEnumerable<Event> Get()
        {
            return events.OrderByDescending(e => e.CreateDateTime);
        } 

        public Event Create(Event newEvent)
        {
            if (newEvent == null) throw new ArgumentNullException("newEvent");

            if(string.IsNullOrWhiteSpace(newEvent.Name)) throw new ArgumentException("Name in newEvent is null or empty");

            newEvent.Id = Utility.GetNextId();
            newEvent.Name = newEvent.Name.Trim();
            newEvent.CreateDateTime = DateTime.Now;
//            newEvent.AttendingFamilies = newEvent.AttendingFamilies.Where(f => f.Count != 0 || f.Expense != 0);
            events.Add(newEvent);
            return newEvent;
        }

        public void Update(Event updatedEvent)
        {
            if (updatedEvent == null) throw new ArgumentNullException("updatedEvent");

            if (string.IsNullOrWhiteSpace(updatedEvent.Name)) throw new ArgumentException("Name in newEvent is null or empty");

            updatedEvent.CreateDateTime = DateTime.Now;

            int index = events.FindIndex(e => e.Id == updatedEvent.Id);
            if(index < 0) throw new ArgumentException(string.Format("evetn does not exist for updating. Id = {0}", updatedEvent.Id));

            events[index] = updatedEvent;
        }

        public void Delete(int eventId)
        {
            events.RemoveAll(e => e.Id == eventId);
        }
    }
}