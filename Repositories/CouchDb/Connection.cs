using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Dental.Repositories.CouchDb
{
    public class Connection
    {
        public static UriBuilder uriBuilder;
        public static HttpClient httpClient;
        public static Connection connection;

        private Connection()
        { }


        public HttpClient HttpClient
        {
            get => httpClient;
        }

        public UriBuilder UriBuilder
        {
            get => uriBuilder;
        }

        public static Connection getInstance()
        {
            if (connection == null)
            {
                connection = new Connection();

                uriBuilder = new UriBuilder();
                uriBuilder.Host = "127.0.0.1";
                uriBuilder.Scheme = "http";
                uriBuilder.Port = 5984;
                uriBuilder.UserName = "admin";
                uriBuilder.Password = "Kolimasova657913";
                uriBuilder.Path = "dental";

                httpClient = new HttpClient();
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(uriBuilder.UserName + ":" + uriBuilder.Password));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", svcCredentials);

                return connection;
            }
            return connection;
        }







    }


}
