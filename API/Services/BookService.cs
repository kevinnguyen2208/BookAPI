using API.Models;
using API.Repositories;
using System.Text.RegularExpressions;

namespace API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookingRepository)
        {
            _bookRepository = bookingRepository;
        }

        public async Task<ServiceResult<IEnumerable<BookViewModel>>> GetAllBooks()
        {
            IEnumerable<BookViewModel> res = await _bookRepository.GetAllBooks();
            if (res == null || !res.Any())
            {
                return ServiceResult<IEnumerable<BookViewModel>>.CreateErrorMessage("No books in database.");
            }
            return new ServiceResult<IEnumerable<BookViewModel>>(res);
        }

        public async Task<ServiceResult<BookViewModel>> GetBookById(int id)
        {
            BookViewModel res = await _bookRepository.GetBookById(id);
            if (res == null)
            {
                return ServiceResult<BookViewModel>.CreateErrorMessage("Book not found.");
            }
            return new ServiceResult<BookViewModel>(res);
        }

        public async Task<ServiceResult<BookViewModel>> CreateBook(BookDto book)
        {
            ServiceResult<BookViewModel> val = await ValidateRequest(book);
            if (val != null) return val;

            BookViewModel create = await _bookRepository.CreateBook(book);
            return new ServiceResult<BookViewModel>(create);
        }

        public async Task<ServiceResult<BookViewModel>> UpdateBook(int id, BookDto book)
        {
            ServiceResult<BookViewModel> val = await ValidateRequest(book, id);
            if (val != null) return val;

            BookViewModel update = await _bookRepository.UpdateBook(id, book);
            return new ServiceResult<BookViewModel>(update);
        }

        /// <summary>
        /// Validate parameters, date format of Published Date, and unique ISBN before creating/updating book
        /// </summary>
        /// <param name="book"></param>
        /// <param name="id">if id is null, meaning create action. Else update action</param>
        /// <returns></returns>
        private async Task<ServiceResult<BookViewModel>> ValidateRequest(BookDto book, int? id = null)
        {
            if (!ValidateParameters(book))
            {
                return ServiceResult<BookViewModel>.CreateErrorMessage("Empty value in request.");
            }

            if (!await ValidateUniqueISBN(book, id))
            {
                return ServiceResult<BookViewModel>.CreateErrorMessage("This ISBN has been stored.");
            }

            return null;
        }


        /// <summary>
        /// Validate all parameters but not be null or empty
        /// </summary>
        private static bool ValidateParameters(BookDto book)
        {
            return !string.IsNullOrEmpty(book.Title)
                && !string.IsNullOrEmpty(book.Author)
                && !string.IsNullOrEmpty(book.ISBN);
        }

        /// <summary>
        /// Valdiate unique ISBN before creating/updating
        /// If it is an update action i.e. id != null, check for different ID as well since updating the same ISBN with the same ID is allowed
        /// </summary>
        private async Task<bool> ValidateUniqueISBN(BookDto book, int? id = null)
        {
            //delegate to filter based on logic
            Func<BookWithIdDto, bool> isbnLookUpLogic;
            if (id != null)
            {
                isbnLookUpLogic = b => b.ISBN == book.ISBN && b.Id != id;
            }
            else
            {
                isbnLookUpLogic = b => b.ISBN == book.ISBN;
            }

            BookWithIdDto res = await _bookRepository.GetBookByISBN(isbnLookUpLogic);
            return res == null;
        }
    }
}
