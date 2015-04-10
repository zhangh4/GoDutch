using System.Collections.Generic;
using GoDutch.Models;

namespace GoDutch.Repository
{
    public interface IEventRepository
    {
        IEnumerable<Event> Get(bool? active = null);
        Event Create(Event newEvent);
        void Update(Event updatedEvent);
        void UpdateEventStatus(int eventId, bool active);
        void Delete(int eventId);
    }
}