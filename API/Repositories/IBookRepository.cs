using API.Models;

namespace API.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookViewModel>> GetAllBooks();
        Task<BookViewModel> GetBookById(int id);
        Task<BookWithIdDto> GetBookByISBN(Func<BookWithIdDto, bool> bookDelegate);
        Task<BookViewModel> CreateBook(BookDto book);
        Task<BookViewModel> UpdateBook(int id, BookDto book);
    }
}
