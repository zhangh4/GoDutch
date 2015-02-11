using System.Collections.Generic;
using GoDutch.Models;

namespace GoDutch.Repository
{
    public interface IEventRepository
    {
        IEnumerable<Event> Get();
        Event Create(Event newEvent);
        void Update(Event updatedEvent);
        void Delete(int eventId);
    }
}