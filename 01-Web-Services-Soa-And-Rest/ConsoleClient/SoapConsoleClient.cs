namespace ConsoleClient
{
    using System;

    using ConsoleClient.ServiceReferenceDistanceCalculator;

    public class SoapConsoleClient
    {
        public static void Main()
        {
            DistanceCalculatorClient client = new DistanceCalculatorClient();
            double result1 = client.CalcDistance(
                new Point { X = 0, Y = 0 },
                new Point { X = 2, Y = 2 });

            Console.WriteLine(result1);

            double result2 = client.CalcDistance(
                new Point { X = 3, Y = 4 },
                new Point { X = -3, Y = -5 });

            Console.WriteLine(result2);
        }
    }
}
