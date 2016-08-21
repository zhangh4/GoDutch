using System.Collections.Generic;

namespace Domain
{
    public interface IExpenseRepository
    {
        Expense Create(Expense newExpense);
        void Update(Expense updatedEvent);
        void Delete(int expenseId);
        IEnumerable<Expense> Get(int eventId);
    }
}
