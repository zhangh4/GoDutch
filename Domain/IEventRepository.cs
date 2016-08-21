using System.Collections.Generic;

namespace Domain
{
    public interface IEventRepository
    {
        IEnumerable<Event> Get(bool? active = null);
        Event Get(int eventId);
        Event CreateOrUpdate(Event thEvent);
        void Delete(int eventId);

//        void Update(Expense updatedExpense);
//        void UpdateEventStatus(int eventId, bool active);
        
    }
}