using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using GoDutch.Utils;

namespace GoDutch.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private IFamilyRepository familyRepo;

        private readonly List<Expense> expenses;

        public ExpenseRepository(IFamilyRepository familyRepo)
        {
            this.familyRepo = familyRepo;
             
            expenses = new List<Expense>()
            {
//                new Expense()
//                {
//                    Id = Utility.GetNextId(), 
//                    Name = "Werewolf",
//                    CreateDateTime = DateTime.Now,
//                    AttendingFamilies = familyRepo.Get().Select(f => new AttendingFamily()
//                    {
//                        Id = f.Id,
//                        Name = f.Name,
//                        Expense = 10,
//                        Count = 3
//                    })
//                }
            };
        }

        public IEnumerable<Expense> Get(int eventId)
        {
            return expenses.OrderByDescending(e => e.CreateDateTime);
        } 

        public Expense Create(Expense newExpense)
        {
            if (newExpense == null) throw new ArgumentNullException("newExpense");

            if(string.IsNullOrWhiteSpace(newExpense.Name)) throw new ArgumentException("Name in newExpense is null or empty");

            newExpense.Id = Utility.GetNextId();
            newExpense.Name = newExpense.Name.Trim();
            newExpense.CreateDateTime = DateTime.Now;
//            newExpense.AttendingFamilies = newExpense.AttendingFamilies.Where(f => f.Count != 0 || f.Expense != 0);
            expenses.Add(newExpense);
            return newExpense;
        }

        public void Update(Expense updatedExpense)
        {
            if (updatedExpense == null) throw new ArgumentNullException("updatedExpense");

            if (string.IsNullOrWhiteSpace(updatedExpense.Name)) throw new ArgumentException("Name in newExpense is null or empty");

            updatedExpense.CreateDateTime = DateTime.Now;

            int index = expenses.FindIndex(e => e.Id == updatedExpense.Id);
            if(index < 0) throw new ArgumentException(string.Format("evetn does not exist for updating. Id = {0}", updatedExpense.Id));

            expenses[index] = updatedExpense;
        }

        public void UpdateEventStatus(int eventId, bool active)
        {
            throw new NotImplementedException();
        }

        public void Delete(int expenseId)
        {
            expenses.RemoveAll(e => e.Id == expenseId);
        }
    }
}