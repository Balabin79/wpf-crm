using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Dental.Models;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dental.Repositories.CouchDb
{
    public class QueryDoc
    {
        public  Connection connection;

        public QueryDoc() 
        {
            this.connection = Connection.getInstance();
        }

       /* public void GetDocument(string documentName)
        {
            String specificUri = connection.Path + "/" + documentName;
            httpClient.

        }*/




        public async Task GetDocument(string documentName)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                connection.UriBuilder.Path += ("/" + documentName);


                HttpResponseMessage response = await connection.HttpClient.GetAsync(connection.UriBuilder.Uri);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                int v = 0;
                //Doctor doctor = JsonSerializer.Deserialize<Doctor>(responseBody);
                JObject user = JObject.Parse(responseBody);


                Doctor doctor = JsonConvert.DeserializeObject<Doctor>(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

    }
}
