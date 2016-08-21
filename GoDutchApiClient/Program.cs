using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace GoDutchApiClient
{
    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://GoDutch/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // delete all events
                List<Event> events = await Execute<List<Event>>(client, HttpMethod.Get, "api/events");
                Task.WaitAll(
                    events.Select(e => Execute<object>(client, HttpMethod.Delete, string.Format("api/events/{0}", e.Id))).ToArray()
                );
                Console.WriteLine("All events deleted");

                // delete all families
                Execute<object>(client, HttpMethod.Delete, "api/families").Wait();
                Console.WriteLine("All families deleted");

                // add families
                var families = new List<Family>()
                {
                    new Family() {Name = "Brayden"},
                    new Family() {Name = "Debra"},
                    new Family() {Name = "Jason"}
                };

                Task.WaitAll(
                    families.Select(family =>
                        Execute(client, HttpMethod.Post, "api/families", family)
                        .ContinueWith(t =>
                        {
                            families.Single(f => f.Name == t.Result.Name).Id = t.Result.Id;
                        })).ToArray()
                    );

                foreach (var family in families)
                {
                    Console.WriteLine(family);
                }
                Console.WriteLine("All families created");

                // add event
                var theEvent = new Event()
                {
                    Active = true,
                    Name = "ski trip",
                    Expenses = new List<Expense>()
                    {
                        new Expense()
                        {
                            Name = "lunch",
                            Attendances = new List<Attendance>()
                            {
                                new Attendance() { Family = families[0], Cost = 11.11m, HeadCount = 0 },
                                new Attendance() { Family = families[1], Cost = 22.22m, HeadCount = 1.5},
                                new Attendance() { Family = families[2], Cost = 33.33m, HeadCount = 2.5},
                            }
                        }
                    }
                };
                theEvent = await Execute(client, HttpMethod.Post, "api/events", theEvent);
                Console.WriteLine("Event created");

                // modify the event right after add
                theEvent.Expenses.First().Attendances.Single(a => a.Family.Name == "Brayden").HeadCount = 5;
                theEvent.Expenses.First()
                    .Attendances.Remove(theEvent.Expenses.First().Attendances.Single(a => a.Family.Name == "Jason"));
                theEvent.Expenses.Add(new Expense()
                {
                    Name = "dinner",
                    Attendances = new List<Attendance>()
                            {
                                new Attendance() { Family = families[0], Cost = 1.11m, HeadCount = 10 },
                                new Attendance() { Family = families[1], Cost = 2.22m, HeadCount = 11.5},
                                new Attendance() { Family = families[2], Cost = 3.33m, HeadCount = 21.5},
                            }
                });
                Execute(client, HttpMethod.Put, string.Format("api/events/{0}", theEvent.Id), theEvent).Wait();
                Console.WriteLine("Event updated");

                // reload the event
                theEvent = await Execute<Event>(client, HttpMethod.Get, string.Format("api/events/{0}", theEvent.Id));
                theEvent.Name = "the greatest skip trip ever";
                theEvent.Expenses.Remove(theEvent.Expenses.First());
                Execute(client, HttpMethod.Put, string.Format("api/events/{0}", theEvent.Id), theEvent).Wait();
                Console.WriteLine("Event reloaded and updated again");
            }
        }

        private static async Task<T> Execute<T>(HttpClient client, HttpMethod method, string uri, T payload = null) where T : class 
        {
            Task<HttpResponseMessage> task = null;
            if (method == HttpMethod.Get)
            {
                task = client.GetAsync(uri);
            }
            else if (method == HttpMethod.Post)
            {
                task = client.PostAsJsonAsync(uri, payload);
            }
            else if (method == HttpMethod.Put)
            {
                task = client.PutAsJsonAsync(uri, payload);
            }
            else if (method == HttpMethod.Delete)
            {
                task = client.DeleteAsync(uri);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid method: {0}", method));
            }

            var response = await task;
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<T>();
            }
            else
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
                      
        }
    }
}
