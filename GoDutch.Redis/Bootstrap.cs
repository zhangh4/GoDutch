using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoDutch.Common.Models;
using StackExchange.Redis;

namespace GoDutch.Redis
{
    public static class Bootstrap
    {
        private static short familyIdValue = 1;
        private static List<Family> seedFamilies = new List<Family>()
        {
            new Family() { Id = familyIdValue++, Name = "Alvin"},
            new Family() { Id = familyIdValue++, Name = "Brayden"},
            new Family() { Id = familyIdValue++, Name = "Cindy"},
            new Family() { Id = familyIdValue++, Name = "Debra"},
            new Family() { Id = familyIdValue++, Name = "Devin"},
            new Family() { Id = familyIdValue++, Name = "Jason"},
            new Family() { Id = familyIdValue++, Name = "Joanna"},
            new Family() { Id = familyIdValue++, Name = "Justin"},
            new Family() { Id = familyIdValue++, Name = "Roger"},
            new Family() { Id = familyIdValue++, Name = "Elaine"},
        }; 

        public static void Run()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            var server = redis.GetServer("localhost:6379");

            Stopwatch watch = Stopwatch.StartNew();
            bool dbInitialized = server.Keys().Any();
            Console.WriteLine("checking any key exists takes " + watch.Elapsed);

            if (dbInitialized)
            {
                Console.WriteLine("DB already initialized, skipping bootstrap...");
                return;
            }

            Console.WriteLine("Bootstrap started...");
            watch = Stopwatch.StartNew();
            foreach (var seedFamily in seedFamilies)
            {
                db.HashSet(FamilyRedisRepository.GetKey(seedFamily.Id), new[] {new HashEntry("name", seedFamily.Name),});
            }
//            for (int i = 0; i < 100000; i++)
//            {
//                db.HashSet(FamilyRedisRepository.GetKey(familyIdValue++), new[] { new HashEntry("name", "Brayden"), });
//            }


             // set correct value for family id key value
            db.StringSet("id.family", familyIdValue);
            Console.WriteLine("Bootstrap completed in " + watch.Elapsed);
        }
    }
}