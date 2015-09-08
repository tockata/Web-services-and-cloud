namespace DistanceCalculatorRestService.Controllers
{
    using System;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using DistanceCalculatorRestService.Models;

    public class CalculatorController : ApiController
    {
        [HttpPost]
        [Route("calcdistance")]
        public double CalcDistance(FormDataCollection data)
        {
            int x1 = int.Parse(data.Get("x1"));
            int y1 = int.Parse(data.Get("y1"));
            int x2 = int.Parse(data.Get("x2"));
            int y2 = int.Parse(data.Get("y2"));
            Point startPoint = new Point { X = x1, Y = y1 };
            Point endPoint = new Point { X = x2, Y = y2 };

            return Math.Sqrt(
                Math.Pow(endPoint.X - startPoint.X, 2) +
                Math.Pow(endPoint.Y - startPoint.Y, 2));
        }
    }
}
