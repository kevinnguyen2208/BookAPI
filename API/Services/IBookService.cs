using API.Models;

namespace API.Services
{
    public interface IBookService
    {
        Task<ServiceResult<IEnumerable<BookViewModel>>> GetAllBooks();
        Task<ServiceResult<BookViewModel>> GetBookById(Guid id);
        Task<ServiceResult<Guid>> CreateBook(BookDto book);
        Task<ServiceResult<Guid>> UpdateBook(Guid id, BookDto book);
    }
}
