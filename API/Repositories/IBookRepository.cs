using API.Models;

namespace API.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookViewModel>> GetAllBooks();
        Task<BookViewModel> GetBookById(Guid id);
        Task<BookWithIdDto> GetBookByISBN(Func<BookWithIdDto, bool> bookDelegate);
        Task<Guid> CreateBook(BookDto book);
        Task<Guid> UpdateBook(Guid id, BookDto book);
    }
}
