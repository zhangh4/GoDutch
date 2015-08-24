using System;
using System.Collections.Generic;
using System.Linq;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using ServiceStack.Redis;

namespace GoDutch.Redis
{
    public class FamilyRedisRepository : RedisRepoBase, IFamilyRepository
    {
        public IEnumerable<Family> Get()
        {
            using (var client = Manager.GetClient())
            {
                return client.As<Family>().GetAll();
            }
        }
    }
}