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

        public async Task<ServiceResult<BookViewModel>> GetBookById(Guid id)
        {
            BookViewModel res = await _bookRepository.GetBookById(id);
            if (res == null)
            {
                return ServiceResult<BookViewModel>.CreateErrorMessage("Book not found.");
            }
            return new ServiceResult<BookViewModel>(res);
        }

        public async Task<ServiceResult<Guid>> CreateBook(BookDto book)
        {
            ServiceResult<Guid> val = await ValidateRequest(book);
            if (val != null) return val;

            Guid id = await _bookRepository.CreateBook(book);
            return new ServiceResult<Guid>(id);
        }

        public async Task<ServiceResult<Guid>> UpdateBook(Guid id, BookDto book)
        {
            ServiceResult<Guid> val = await ValidateRequest(book, id);
            if (val != null) return val;

            await _bookRepository.UpdateBook(id, book);
            return new ServiceResult<Guid>(id);
        }

        /// <summary>
        /// Validate parameters, date format of Published Date, and unique ISBN before creating/updating book
        /// </summary>
        /// <param name="book"></param>
        /// <param name="id">if id is null, meaning create action. Else update action</param>
        /// <returns></returns>
        private async Task<ServiceResult<Guid>> ValidateRequest(BookDto book, Guid? id = null)
        {
            if (!ValidateParameters(book))
            {
                return ServiceResult<Guid>.CreateErrorMessage("Empty value in request.");
            }

            if (!ValidateDateFormat(book.PublishedDate))
            {
                return ServiceResult<Guid>.CreateErrorMessage("Published Date must be in yyyy-mm-dd format.");
            }

            if (!await ValidateUniqueISBN(book, id))
            {
                return ServiceResult<Guid>.CreateErrorMessage("This ISBN has been stored.");
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
                && !string.IsNullOrEmpty(book.ISBN)
                && !string.IsNullOrEmpty(book.PublishedDate);
        }

        /// <summary>
        /// Validate Published Date to be of format yyyy-mm-dd
        /// </summary>
        private static bool ValidateDateFormat(string date)
        {
            string pattern = @"^\d{4}\-(0[1-9]|1[012])\-(0[1-9]|[12][0-9]|3[01])$";
            bool isValidTimeFormat = Regex.IsMatch(date, pattern);
            return isValidTimeFormat;
        }

        /// <summary>
        /// Valdiate unique ISBN before creating/updating
        /// If it is an update action i.e. id != null, check for different ID as well since updating the same ISBN with the same ID is allowed
        /// </summary>
        private async Task<bool> ValidateUniqueISBN(BookDto book, Guid? id = null)
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
