using Services.Contracts;

namespace Services.Abstractions
{
    public interface IBookService<T> where T : class
    {
        Task<T?> GetBookByIdAsync(long? id);
    }
}
