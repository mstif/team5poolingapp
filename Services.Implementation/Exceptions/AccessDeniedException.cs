namespace Services.Implementation.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string name)
            : base($"You do not have permission to access this {name}.") { }
    }
}
