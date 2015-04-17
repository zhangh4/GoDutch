using System.Collections.Generic;
using GoDutch.Models;

namespace GoDutch.Repository
{
    public interface IEventRepository
    {
        IEnumerable<Event> Get(bool? active = null);
        Event Get(int eventId);
        Event Create(string eventName);
        void Delete(int eventId);

//        void Update(Expense updatedExpense);
//        void UpdateEventStatus(int eventId, bool active);
        
    }
}