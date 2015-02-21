using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoDutch.Models;
using GoDutch.Utils;

namespace GoDutch.Repository
{
    public class FamilyRepository : IFamilyRepository
    {
        private Family[] families = new[]
        {
            new Family() { Id = Utility.GetNextId(), Name = "Brayden"},
            new Family() { Id = Utility.GetNextId(), Name = "Jason" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Debra" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Alvin" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Roger" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Devin" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Justin" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Cindy" }, 
            new Family() { Id = Utility.GetNextId(), Name = "Joanna" }, 
        };


        public IEnumerable<Family> Get() { return families.OrderBy(f => f.Name); } 
    }
}