namespace Infrastructure.Exceptions
{
    public class DeliveryPathException : ExceptionBase
    {
        public DeliveryPathException(string message, string droneId)
            : base(message, droneId)
        {
        }
    }
}
