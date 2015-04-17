using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoDutch.Models;

namespace GoDutch.Repository
{
    public interface IExpenseRepository
    {
        Expense Create(Expense newExpense);
        void Update(Expense updatedEvent);
        void Delete(int expenseId);
        IEnumerable<Expense> Get(int eventId);
    }
}
