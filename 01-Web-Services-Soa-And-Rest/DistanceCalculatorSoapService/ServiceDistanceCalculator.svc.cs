namespace DistanceCalculatorSoapService
{
    using System;

    public class ServiceDistanceCalculator : IDistanceCalculator
    {
        public double CalcDistance(Point startPoint, Point endPoint)
        {
            return Math.Sqrt(
                Math.Pow(endPoint.X - startPoint.X, 2) +
                Math.Pow(endPoint.Y - startPoint.Y, 2));
        }
    }
}
