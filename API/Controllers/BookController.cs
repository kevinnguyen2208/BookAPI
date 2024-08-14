using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookingService)
        {
            _bookService = bookingService;
        }

        [HttpGet]
        [StatusCodeHandler(Return200ForInvalid = true)]
        public async Task<ServiceResult<IEnumerable<BookViewModel>>> GetAllBooks()
        {
            return await _bookService.GetAllBooks();
        }

        [HttpGet("{id}")]
        [StatusCodeHandler]
        public async Task<ServiceResult<BookViewModel>> GetBookById(int id)
        {
            return await _bookService.GetBookById(id);
        }

        [HttpPost]
        [StatusCodeHandler]
        public async Task<ServiceResult<BookViewModel>> CreateBook([FromBody] BookDto request)
        {
            return await _bookService.CreateBook(request);
        }

        [HttpPut("{id}")]
        [StatusCodeHandler]
        public async Task<ServiceResult<BookViewModel>> UpdateBook(int id, [FromBody] BookDto request)
        {
            return await _bookService.UpdateBook(id, request);
        }
    }
}
