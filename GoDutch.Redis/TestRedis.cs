using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using NUnit.Framework;
using ServiceStack.Redis;

namespace GoDutch.Redis
{
    [TestFixture]
    public class TestRedis
    {
        private string host = "localhost";
        private int port = 6379;
        private FamilyRedisRepository familyRepo;
        private EventRedisRepository eventRepo;
        private IRedisClientsManager manager;

        [SetUp]
        public void Init()
        {
            manager = new BasicRedisClientManager("localhost");
            familyRepo = new FamilyRedisRepository() {Manager = manager};
            eventRepo = new EventRedisRepository() {Manager = manager};
            

            manager.GetClient().FlushAll();
        }

        [Test]
        public void TestBootstrap()
        {
            new Bootstrap() {Manager = manager}.Run();
        }

        [Test]
        public void TestFamilyRepoGet()
        {
            Stopwatch watch = Stopwatch.StartNew();
            var familites = familyRepo.Get().ToList();
            Console.WriteLine("Families retrieved in " + watch.Elapsed);
            familites.ForEach(System.Console.WriteLine);
        }

        [Test]
        public void TestEventRepo()
        {
            var event1 = eventRepo.Create("Test Event 1");
            var event2 = eventRepo.Create("Test Event 2");

            Assert.AreEqual(event1, eventRepo.Get(event1.Id));
            Assert.AreEqual(event2, eventRepo.Get(event2.Id));

            eventRepo.Delete(event1.Id);
            eventRepo.Delete(event2.Id);

            Assert.IsNull(eventRepo.Get(event1.Id));
            Assert.IsNull(eventRepo.Get(event2.Id));
        }

        [Test]
        public void TestExpenseRepo()
        {
            var event1 = new Event() { Name = "Test Event 1"};
            var expense1 = new Expense() { Name = "Test Expense 1", AttendingFamilies = new []{ new AttendingFamily() {Id = 1, Name = "Alvin", Count = 2.5, Expense = 40.5m}}};
            var expense2 = new Expense() { Name = "Test Expense 2" };
            event1.Expenses = new HashSet<Expense>(new[] {expense1, expense2});
            event1 = eventRepo.CreateOrUpdate(event1);
            Assert.AreEqual(event1, eventRepo.Get(event1.Id));

        }

        [Test]
        public void TestDatetime()
        {
            string format = "O";
            var now = DateTime.Now;
            Assert.AreEqual(now, DateTime.Parse(now.ToString(format)));
        }
    }
}