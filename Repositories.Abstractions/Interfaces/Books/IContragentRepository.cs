using ApplicationCore.Books;
using Services.Contracts;

namespace ApplicationCore.Interfaces.Books
{
    public interface IContragentRepository : IBookRepository<Contragent>
    {
        Task<List<Contragent>> GetPagedAsync(ContragentFilterDto filterDto);
    }
}
