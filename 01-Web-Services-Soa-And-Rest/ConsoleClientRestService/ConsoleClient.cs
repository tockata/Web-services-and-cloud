namespace ConsoleClientRestService
{
    using System;

    using RestSharp;

    public class ConsoleClient
    {
        public static void Main()
        {
            var client = new RestClient("http://localhost:60759");
            var request = new RestRequest("calcdistance", Method.POST);
            request.AddParameter("x1", 0);
            request.AddParameter("y1", 0);
            request.AddParameter("x2", 2);
            request.AddParameter("y2", 2);

            var response = client.Execute(request);
            Console.WriteLine(response.Content);

            var request2 = new RestRequest("calcdistance", Method.POST);
            request2.AddParameter("x1", 3);
            request2.AddParameter("y1", 4);
            request2.AddParameter("x2", -3);
            request2.AddParameter("y2", -5);

            var response2 = client.Execute(request2);
            Console.WriteLine(response2.Content);
        }
    }
}