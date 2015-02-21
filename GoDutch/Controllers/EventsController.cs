﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GoDutch.Models;
using GoDutch.Repository;

namespace GoDutch.Controllers
{
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
    }
}