namespace DistanceCalculatorSoapService
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IDistanceCalculator
    {
        [OperationContract]
        double CalcDistance(Point startPoint, Point endPoint);
    }
}
