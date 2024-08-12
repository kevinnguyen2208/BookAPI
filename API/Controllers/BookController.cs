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
        public async Task<IActionResult> GetAllBooks()
        {
            ServiceResult<IEnumerable<BookViewModel>> res = await _bookService.GetAllBooks();
            return HandleReturn(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            ServiceResult<BookViewModel> res = await _bookService.GetBookById(id);
            return HandleReturn(res);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookDto request)
        {
            ServiceResult<Guid> res = await _bookService.CreateBook(request);
            return HandleReturn(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookDto request)
        {
            ServiceResult<Guid> res = await _bookService.UpdateBook(id, request);
            return HandleReturn(res);
        }

        /// <summary>
        /// Generic function to handle the same return logic
        /// </summary>
        private IActionResult HandleReturn<T>(ServiceResult<T> res)
        {
            if (res.Validation == ValidationTypes.Invalid)
            {
                return BadRequest(res.Message);
            }
            return Ok(res.Value);
        }
    }
}
