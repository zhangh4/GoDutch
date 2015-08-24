using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using ServiceStack.Redis;

namespace GoDutch.Redis
{
    public abstract class RedisRepoBase
    {
        [Dependency]
        public IRedisClientsManager Manager { get; set; }
    }
}
