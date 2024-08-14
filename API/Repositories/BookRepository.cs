using API.Data;
using API.Models;
using API.Mappers;
using Microsoft.EntityFrameworkCore;
namespace API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookViewModel>> GetAllBooks()
        {
            return (await _context.Books.ToArrayAsync()).ToViewModel();
        }

        public async Task<BookViewModel> GetBookById(int id)
        {
            return (await _context.Books.FindAsync(id)).ToViewModel();
        }

        /// <summary>
        /// Check for delegate based on create/update operation
        /// NOTE: easier to use EntityFramework FirstOrDefaultAsync, however wants to demonstrate delegate and EF doesn't take in delegate
        /// Hence need to query on client side
        /// </summary>
        public async Task<BookWithIdDto> GetBookByISBN(Func<BookWithIdDto, bool> bookDelegate)
        {
            List<BookWithIdDto> allBooks = await _context.Books.ToListAsync();
            return allBooks.Where(bookDelegate).FirstOrDefault();
        }

        public async Task<BookViewModel> CreateBook(BookDto request)
        {
            BookWithIdDto book = new BookWithIdDto()
            {
                Author = request.Author,
                Title = request.Title,
                ISBN = request.ISBN,
                PublishedDate = request.PublishedDate,
                CreatedOn = DateTime.UtcNow
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book.ToViewModel();
        }

        public async Task<BookViewModel> UpdateBook(int id, BookDto request)
        {
            BookWithIdDto book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                throw new KeyNotFoundException($"Book with Id {id} not found.");
            }

            book.Author = request.Author;
            book.Title = request.Title;
            book.ISBN = request.ISBN;
            book.PublishedDate = request.PublishedDate;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book.ToViewModel();
        }
    }
}
