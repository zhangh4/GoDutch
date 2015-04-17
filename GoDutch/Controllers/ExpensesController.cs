using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GoDutch.Models;
using GoDutch.Repository;

namespace GoDutch.Controllers
{
    public class ExpensesController : ApiController
    {
        private IExpenseRepository repo;

        public ExpensesController(IExpenseRepository repo)
        {
            this.repo = repo;
        }

        
        // POST: api/Expenses
        public Expense Post([FromBody]Expense value)
        {
            return repo.Create(value);
        }

        // PUT: api/Expenses/5
        public void Put(int id, [FromBody]Expense value)
        {
            repo.Update(value);
        }

        // DELETE: api/Expenses/5
        public void Delete(int id)
        {
            repo.Delete(id);
        }
    }
}
