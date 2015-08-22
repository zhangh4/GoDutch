using System.Collections.Generic;
using GoDutch.Common.Models;

namespace GoDutch.Common.Repository
{
    public interface IExpenseRepository
    {
        Expense Create(Expense newExpense);
        void Update(Expense updatedExpense);
        void Delete(int expenseId);
        IEnumerable<Expense> Get(int eventId);
    }
}
