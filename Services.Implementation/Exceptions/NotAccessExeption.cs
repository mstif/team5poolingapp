namespace Services.Implementation.Exceptions
{
    public class NotAccessExeption : Exception
    {
        public NotAccessExeption(string name, object key)
            : base($"{name} [{key}] нет доступа к объекту.") { }
    }
}
