using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GoDutch.Models;

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

                // HTTP GET
                HttpResponseMessage response = await client.GetAsync("api/events/16");
                if (response.IsSuccessStatusCode)
                {
                    Event result = await response.Content.ReadAsAsync<Event>();
                    Console.WriteLine("{0}", result.Name);
                }
                else
                {
                    Console.WriteLine(response);
                }

            }
        }
    }
}
