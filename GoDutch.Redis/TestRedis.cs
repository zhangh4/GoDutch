using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoDutch.Common.Models;
using GoDutch.Common.Repository;
using NUnit.Framework;
using StackExchange.Redis;

namespace GoDutch.Redis
{
    [TestFixture]
    public class TestRedis
    {
        private string host = "localhost";
        private int port = 6379;
        private IConnectionMultiplexer connection;
        private IFamilyRepository familyRepo;
        private IEventRepository eventRepo;
        private IExpenseRepository expenseRepo;

        [SetUp]
        public void Init()
        {
            connection = ConnectionMultiplexer.Connect(host);
            familyRepo = new FamilyRedisRepository(connection, host, port);
            eventRepo = new EventRedisRepository(connection, host, port);
            expenseRepo = new ExpenseRedisRepository(connection, host, port);
           // todo: use unity 
            ((EventRedisRepository)eventRepo).ExpenseRepository = expenseRepo;
        }

        [Test]
        public void TestBootstrap()
        {
            Bootstrap.Run();
        }

        [Test]
        public void TestFamilyRepoGet()
        {
            Stopwatch watch = Stopwatch.StartNew();
            var familites = familyRepo.Get().ToList();
            Console.WriteLine("Families retrieved in " + watch.Elapsed);
//            familites.ForEach(System.Console.WriteLine);
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
            var event1 = eventRepo.Create("Test Event 1");
            var expense1 = expenseRepo.Create(new Expense() { Name = "Test Expense 1", EventId = event1.Id });
            var expense2 = expenseRepo.Create(new Expense() { Name = "Test Expense 2", EventId = event1.Id });
            event1.Expenses = new HashSet<Expense>(new[] {expense1, expense2});
            Assert.AreEqual(event1, eventRepo.Get(event1.Id));

            expenseRepo.Delete(expense1.Id);
            var eventAfterExpenseDeleted = eventRepo.Get(event1.Id);
            Assert.AreEqual(1, eventAfterExpenseDeleted.Expenses.Count());
            Assert.AreEqual(expense2.Id, eventAfterExpenseDeleted.Expenses.ToArray()[0].Id);

            eventRepo.Delete(event1.Id);
            Assert.IsNull(eventRepo.Get(event1.Id));
            Assert.IsEmpty(expenseRepo.Get(event1.Id));
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