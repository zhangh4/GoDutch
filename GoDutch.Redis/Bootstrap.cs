using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoDutch.Common.Models;
using ServiceStack.Redis;


namespace GoDutch.Redis
{
    public class Bootstrap : RedisRepoBase
    {
        private static short familyIdValue = 1;

        private static List<Family> seedFamilies = new List<Family>()
        {
            new Family() {Name = "Alvin"},
            new Family() {Name = "Brayden"},
            new Family() {Name = "Cindy"},
            new Family() {Name = "Debra"},
            new Family() {Name = "Devin"},
            new Family() {Name = "Jason"},
            new Family() {Name = "Joanna"},
            new Family() {Name = "Justin"},
            new Family() {Name = "Roger"},
            new Family() {Name = "Elaine"},
        };

        public void Run()
        {
            Stopwatch watch = Stopwatch.StartNew();
            using (var client = Manager.GetClient())
            {
                var familyClient = client.As<Family>();
                bool dbInitialized = familyClient.GetRandomKey() != null;
                Console.WriteLine("checking any key exists takes " + watch.Elapsed);

                if (dbInitialized)
                {
                    Console.WriteLine("DB already initialized, skipping bootstrap...");
                    return;
                }

                Console.WriteLine("Bootstrap started...");
                watch = Stopwatch.StartNew();
                seedFamilies.ForEach(f => f.Id = (int) familyClient.GetNextSequence());
                familyClient.StoreAll(seedFamilies);

                Console.WriteLine("Bootstrap completed in " + watch.Elapsed);
            }
        }
    }
}