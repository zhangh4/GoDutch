using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Domain;

namespace GoDutch.Controllers
{
//    [Authorize(Users = @"khu9323")]
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
        public Event Get(int id)
        {
            var principal = Thread.CurrentPrincipal;
            return repo.Get(id);
        }

        // POST: api/Events
        public Event Post([FromBody]Event newEvent)
        {
            return repo.CreateOrUpdate(newEvent);
        }

        // PUT: api/Events/5
        public Event Put(int id, [FromBody]Event value)
        {
            return repo.CreateOrUpdate(value);
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
