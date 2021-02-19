namespace Infrastructure.Exceptions
{
    public class TransportException : ExceptionBase
    {
        public TransportException(string message, string droneId)
            : base(message, droneId)
        {
        }
    }
}
