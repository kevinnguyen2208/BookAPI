using API.Models;

namespace API.Services
{
    public interface IBookService
    {
        Task<ServiceResult<IEnumerable<BookViewModel>>> GetAllBooks();
        Task<ServiceResult<BookViewModel>> GetBookById(int id);
        Task<ServiceResult<BookViewModel>> CreateBook(BookDto book);
        Task<ServiceResult<BookViewModel>> UpdateBook(int id, BookDto book);
    }
}
