using Microsoft.AspNetCore.Mvc;

namespace BookApiChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> _books = new(); //in memory book storage list

        private static int _nextBookId = 1;

        private readonly ILogger<BooksController> _logger;

        public BooksController(ILogger<BooksController> logger)
        {
            _logger = logger;
        }

        //Create a new unpublished book with an ID, Title, Planned Status
        [HttpPost]
        public ActionResult<Book> CreateBook([FromBody] string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title is required.");
            }

            var book = new Book
            {
                BookID = _nextBookId++,  //Auto-increment ID
                BookTitle = title,
                PublicationStatus = "Planned",  //Default to "Planned"
                PublicationDate = null  //Not published yet
            };

            _books.Add(book);

            return CreatedAtAction(nameof(GetBooks), new { id = book.BookID }, book);
        }

        //Publish a book by updating the status to Published
        [HttpPost("{id}/publish")]
        public ActionResult<Book> PublishBook(int id)
        {
            var book = _books.FirstOrDefault(book => book.BookID == id);

            if (book == null) return NotFound();

            if (book.PublicationStatus == "Published")
                return BadRequest("The book is already published.");

            book.PublicationStatus = "Published";
            book.PublicationDate = DateTime.Now;

            return Ok(book);
        }

        //List all books showing titles and publication statuses
        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {

            var booksWithStatus = _books.Select(book => new
            {

                BookTitle = book.BookTitle,
                PublicationStatus = book.PublicationStatus

            });

            return Ok(booksWithStatus);
        }
    }
}

