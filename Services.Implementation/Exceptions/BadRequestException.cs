namespace Services.Implementation.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string errorMsg)
            : base(errorMsg) { }
    }
}
