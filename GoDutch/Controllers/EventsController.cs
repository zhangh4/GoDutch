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
    [RoutePrefix("api/events")]
    public class EventsController : ApiController
    {
        private IEventRepository repo;

        public EventsController(IEventRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/Events
        public IEnumerable<Event> Get()
        {
            return repo.Get();
        }

        [Route("active")]
        public IEnumerable<Event> GetActive()
        {
            return repo.Get(active:true);
        }

        [Route("inactive")]
        public IEnumerable<Event> GetInactive()
        {
            return repo.Get(active:false);
        }

        // GET: api/Events/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Events
        public Event Post([FromBody]Event value)
        {
            return repo.Create(value);
        }

        // PUT: api/Events/5
        public void Put(int id, [FromBody]Event value)
        {
            repo.Update(value);
        }

        // DELETE: api/Events/5
        public void Delete( int id)
        {
            repo.Delete(id);
        }

        public void Delete(int[] ids)
        {
            Console.WriteLine(ids);
        }
    }
}
