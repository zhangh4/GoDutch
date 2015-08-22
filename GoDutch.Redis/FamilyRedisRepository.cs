using System;
using System.Collections.Generic;
using System.Linq;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using StackExchange.Redis;

namespace GoDutch.Redis
{
    public class FamilyRedisRepository : RedisRepositoryBase, IFamilyRepository
    {
        private static string FamilyKeyPrefix = "family:";

        public static string GetKey(int id)
        {
            return string.Format("{0}{1}", FamilyKeyPrefix, id);
        }

        public FamilyRedisRepository(IConnectionMultiplexer connection, string host, int port) : base(connection, host, port)
        {
        }

        public IEnumerable<Family> Get()
        {
            var db = connection.GetDatabase();
            var server = connection.GetServer(string.Format("{0}:{1}", host, port));
            var familyKeys = server.Keys(0, string.Format("{0}*", FamilyKeyPrefix), 10);
            return 
                familyKeys.Select(familyKey => 
                    new Family() { 
                        Id = Convert.ToInt32(familyKey.ToString().Substring(FamilyKeyPrefix.Length)), 
                        Name = db.HashGet(familyKey, "name")});
        }
    }
}