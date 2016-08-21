using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Domain;

namespace GoDutch.Controllers
{
    public class FamiliesController : ApiController
    {
        private IFamilyRepository repo;

        public FamiliesController(IFamilyRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/Family
        public IEnumerable<Family> Get()
        {
            return repo.Get();
        }

        // GET: api/Family/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Family
        public Family Post(Family family)
        {
            return repo.CreateOrUpdate(family);
        }

        // PUT: api/Family/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Family/5
        public void Delete()
        {
            repo.DeleteAll();
        }
    }
}
