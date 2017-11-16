using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TmdbClient
{
    public class DataObject
    {
        public string Name { get; set; }
    }

    public class Class1
    {
        private const string URL = "https://sub.domain.com/objects.json";
        private string urlParameters = "?api_key=30c55e488309018846212ea4e39623cf";

        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
                foreach (var d in dataObjects)
                {
                    Console.WriteLine("{0}", d.Name);
                }
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
}
