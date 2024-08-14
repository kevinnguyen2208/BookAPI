using API.Mappers;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace APITest
{
    public class BookServiceTest
    {
        private readonly ServiceCollection SC;
        private readonly ServiceProvider SP;
        private readonly IBookService _service;

        public BookServiceTest()
        {
            SC = new ServiceCollection();
            SC.AddScoped<IBookService, BookService>();
            SetupBookRepository();
            SP = SC.BuildServiceProvider();
            _service = SP.GetRequiredService<IBookService>();
        }

        private Dictionary<int, BookWithIdDto> BookStorage = new Dictionary<int, BookWithIdDto>();
        private int IDCount = 0;

        /// <summary>
        /// Mock book repository and its functions
        /// </summary>
        private void SetupBookRepository()
        {
            Mock<IBookRepository> mock = new Mock<IBookRepository>(MockBehavior.Strict);

            _ = mock.Setup(s => s.GetAllBooks())
                .ReturnsAsync(BookStorage.Values.ToViewModel);

            _ = mock.Setup(s => s.GetBookById(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    if (BookStorage.TryGetValue(id, out var book))
                    {
                        return book.ToViewModel();
                    }
                    return null;
                });

            _ = mock.Setup(s => s.GetBookByISBN(It.IsAny<Func<BookWithIdDto, bool>>()))
                .ReturnsAsync((Func<BookWithIdDto, bool> bookLogic) =>
                {
                    return BookStorage.Values.FirstOrDefault(f => bookLogic(f));
                });

            _ = mock.Setup(s => s.CreateBook(It.IsAny<BookDto>()))
                .ReturnsAsync((BookDto request) =>
                {
                    BookWithIdDto book = new BookWithIdDto()
                    {
                        Id = IDCount++,
                        Title = request.Title,
                        Author = request.Author,
                        ISBN = request.ISBN,
                        PublishedDate = request.PublishedDate
                    };

                    BookStorage.Add(book.Id, book);
                    return book.ToViewModel();
                });

            _ = mock.Setup(s => s.UpdateBook(It.IsAny<int>(), It.IsAny<BookDto>()))
                .ReturnsAsync((int id, BookDto request) =>
                {
                    BookWithIdDto book = new BookWithIdDto()
                    {
                        Id = id,
                        Title = request.Title,
                        Author = request.Author,
                        ISBN = request.ISBN,
                        PublishedDate = request.PublishedDate
                    };

                    BookStorage[id] = book;
                    return book.ToViewModel();
                });

            SC.AddScoped(_ => mock.Object);
        }

        [Theory]
        [InlineData(null, "Valid Title", "1234567890", false)]
        [InlineData("Valid Author", null, "1234567890", false)]
        [InlineData("Valid Author", "Valid Title", null, false)]
        [InlineData("", "Valid Title", "1234567890", false)]
        [InlineData("Valid Author", "", "1234567890", false)]
        [InlineData("Valid Author", "Valid Title", "", false)]
        [InlineData("Valid Author", "Valid Title", "1234567890", true)]

        public async Task ValidateParameters(string? author, string? title, string? isbn, bool isValid)
        {
            BookDto book = new BookDto()
            {
                Author = author,
                Title = title,
                ISBN = isbn,
                PublishedDate = DateTime.UtcNow
            };
            ServiceResult<BookViewModel> res = await _service.CreateBook(book);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, res.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.Invalid, res.Validation);
            }
        }

        [Theory]
        [InlineData("0", false)]
        [InlineData("1", true)]
        public async Task ValidateUniqueISBNWhenCreate(string isbn, bool isValid)
        {
            BookWithIdDto book = new BookWithIdDto()
            {
                Author = "a",
                Title = "t",
                ISBN = "0",
                PublishedDate = DateTime.UtcNow
            };
            await _service.CreateBook(book);

            book.ISBN = isbn;
            ServiceResult<BookViewModel> res = await _service.CreateBook(book);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, res.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.Invalid, res.Validation);
            }
        }

        [Theory]
        [InlineData("0", false)]
        [InlineData("2", true)]
        public async Task ValidateUniqueISBNWhenUpdate(string isbn, bool isValid)
        {
            BookWithIdDto book = new BookWithIdDto()
            {
                Author = "a",
                Title = "t",
                ISBN = "0",
                PublishedDate = DateTime.UtcNow
            };
            BookWithIdDto book2 = new BookWithIdDto()
            {
                Author = "a2",
                Title = "t2",
                ISBN = "2",
                PublishedDate = DateTime.UtcNow
            };
            ServiceResult<BookViewModel> create = await _service.CreateBook(book);
            ServiceResult<BookViewModel> create2 = await _service.CreateBook(book2);

            book2.ISBN = isbn;
            ServiceResult<BookViewModel> res = await _service.UpdateBook(create2.Value.Id, book2);
            if (isValid)
            {
                Assert.Equal(ValidationTypes.None, res.Validation);
            }
            else
            {
                Assert.Equal(ValidationTypes.Invalid, res.Validation);
            }
        }

        [Fact]
        public async Task CreateBook()
        {
            BookDto book = new BookDto()
            {
                Author = "a",
                Title = "t",
                ISBN = "i",
                PublishedDate = DateTime.UtcNow
            };
            ServiceResult<BookViewModel> res = await _service.CreateBook(book);
            Assert.NotNull(BookStorage[res.Value.Id]);
        }

        [Fact]
        public async Task UpdateBook()
        {
            int id = 0;
            BookWithIdDto book = new BookWithIdDto()
            {
                Id = id,
                Author = "a",
                Title = "t",
                ISBN = "i",
                PublishedDate = DateTime.UtcNow
            };
            BookStorage.Add(id, book);

            string isbn = "1";
            book.ISBN = isbn;
            await _service.UpdateBook(id, book);
            Assert.Equal(isbn, BookStorage[id].ISBN);
        }

        [Fact]
        public async Task GetAllBooks()
        {
            BookDto book = new BookDto()
            {
                Author = "a",
                Title = "t",
                ISBN = "0",
                PublishedDate = DateTime.UtcNow
            };
            await _service.CreateBook(book);

            BookDto book2 = new BookDto()
            {
                Author = "b",
                Title = "f",
                ISBN = "1",
                PublishedDate = DateTime.UtcNow
            };
            await _service.CreateBook(book2);

            ServiceResult<IEnumerable<BookViewModel>> res = await _service.GetAllBooks();
            Assert.Equal(2, res.Value.Count());
            Assert.Collection(res.Value,
                e =>
                {
                    Assert.Equal("0", e.ISBN);
                },
                e =>
                {
                    Assert.Equal("1", e.ISBN);
                }
            );
        }

        [Fact]
        public async Task GetBookById()
        {
            BookDto book = new BookDto()
            {
                Author = "a",
                Title = "t",
                ISBN = "223123",
                PublishedDate = DateTime.UtcNow
            };
            ServiceResult<BookViewModel> create = await _service.CreateBook(book);

            ServiceResult<BookViewModel> res = await _service.GetBookById(create.Value.Id);
            Assert.Equal(create.Value.Id, res.Value.Id);
            Assert.Equal("223123", res.Value.ISBN);
        }
    }
}