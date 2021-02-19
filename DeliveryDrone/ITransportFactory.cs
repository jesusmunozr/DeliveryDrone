namespace DeliveryDrone
{
    public interface ITransportFactory
    {
        ITransport CreateTransport(TransportTypes transportType);
    }
}
