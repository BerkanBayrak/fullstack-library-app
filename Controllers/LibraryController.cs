using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;
using LibraryApi.Models;
using LibraryApi.Models.DTOs;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LibraryController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            var newBook = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                Height = request.Height,
                Width = request.Width,
                Type = request.Type,
                BorrowerId = string.Empty,
                Status = BookStatus.InDepot
            };

            // If shelf placement info is valid (non-null and non-zero)
            if (!string.IsNullOrEmpty(request.Label) && request.Position.HasValue && request.Position.Value > 0)
            {
                var shelf = await _context.Shelves
                    .Include(s => s.Books)
                    .Include(s => s.Bookshelf)
                    .FirstOrDefaultAsync(s =>
                        s.Position == request.Position &&
                        s.Bookshelf.Label == request.Label);

                if (shelf != null)
                {
                    // Check height fit
                    if (newBook.Height > shelf.Height)
                        return BadRequest("Book is too tall for the selected shelf.");

                    // Check width fit
                    int usedWidth = shelf.Books.Sum(b => b.Width);
                    if (usedWidth + newBook.Width > shelf.Width)
                        return BadRequest("Not enough width space on the shelf.");

                    newBook.ShelfId = shelf.Id;
                    newBook.Status = BookStatus.OnShelf;
                }
                else
                {
                    return BadRequest("Shelf not found with the provided label and position.");
                }
            }

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return Ok(newBook);
        }
        [HttpPost("AddBookshelf")]
        public async Task<IActionResult> AddBookshelf([FromBody] AddBookshelfRequest request)
        {
            if (request == null || request.ShelfHeight <= 0)
                return BadRequest("Invalid request or shelf height.");

            if (await _context.Bookshelves.AnyAsync(b => b.Label == request.Label))
                return BadRequest("A bookshelf with this label already exists.");

            if (request.Height % request.ShelfHeight != 0)
                return BadRequest("Shelf height must evenly divide bookshelf height.");

            var bookshelf = new Bookshelf
            {
                Label = request.Label,
                Height = request.Height,
                Width = request.Width
            };

            int numberOfShelves = request.Height / request.ShelfHeight;

            for (int i = 0; i < numberOfShelves; i++)
            {
                var shelf = new Shelf
                {
                    Height = request.ShelfHeight,
                    Width = request.Width,
                    Position = i + 1,
                    Bookshelf = bookshelf
                };
                bookshelf.Shelves.Add(shelf);
            }

            _context.Bookshelves.Add(bookshelf);
            await _context.SaveChangesAsync();

            return Ok(bookshelf);
        }

        [HttpGet("BooksOnShelf")]
        public IActionResult GetBooksOnShelf(string label, int position)
        {
            var shelf = _context.Shelves
                .Include(s => s.Books)
                .Include(s => s.Bookshelf)
                .FirstOrDefault(s =>
                    s.Bookshelf.Label == label &&
                    s.Position == position);

            if (shelf == null)
                return NotFound("Shelf not found.");

            return Ok(shelf.Books);
        }
        [HttpDelete("RemoveBookByTitle")]
        public async Task<IActionResult> RemoveBookByTitle(string title)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Title == title && b.Status != BookStatus.Borrowed);

            if (book == null)
                return NotFound("Book not found or currently borrowed.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok($"Removed book: {book.Title}");
        }


        [HttpPost("MoveBookByTitle")]
        public async Task<IActionResult> MoveBookByTitle(string title, string? label, int position)
        {
            var book = await _context.Books
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(b => b.Title == title);

            if (book == null)
                return NotFound("Book not found.");
            if (book.Status == BookStatus.Borrowed)
                return BadRequest("Cannot move a borrowed book.");

            if (string.IsNullOrEmpty(label) || position <= 0)
            {
                // Move to depot
                book.ShelfId = null;
                book.Status = BookStatus.InDepot;
            }
            else
            {
                var shelf = await _context.Shelves
                    .Include(s => s.Books)
                    .Include(s => s.Bookshelf)
                    .FirstOrDefaultAsync(s =>
                        s.Bookshelf.Label == label &&
                        s.Position == position);

                if (shelf == null)
                    return NotFound("Shelf not found.");

                // Check height and width fit
                if (book.Height > shelf.Height)
                    return BadRequest("Book is too tall for the shelf.");

                int usedWidth = shelf.Books.Sum(b => b.Width);
                if (usedWidth + book.Width > shelf.Width)
                    return BadRequest("Not enough width on the shelf.");

                book.ShelfId = shelf.Id;
                book.Status = BookStatus.OnShelf;
            }

            await _context.SaveChangesAsync();
            return Ok($"Moved book '{book.Title}' successfully.");
        }



        [HttpGet("DepotBooks")]
        public IActionResult GetDepotBooks()
        {
            var depotBooks = _context.Books
                .Where(b => b.Status == BookStatus.InDepot)
                .ToList();

            return Ok(depotBooks);

        }

        [HttpGet("AllBooks")]
        public IActionResult GetAllBooks()
        {
            var allBooks = _context.Books
                .Include(b => b.Shelf)
                .ThenInclude(s => s.Bookshelf)
                .ToList();

            return Ok(allBooks);
        }

        [HttpGet("SearchBooks")]
        public IActionResult SearchBooks(string? title = null, string? author = null, string? genre = null)
        {
            var query = _context.Books
                .Include(b => b.Shelf)
                .ThenInclude(s => s.Bookshelf)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                 query = query.Where(b => b.Title.Contains(title));

            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author.Contains(author));

            if (!string.IsNullOrEmpty(genre))
                 query = query.Where(b => b.Genre.Contains(genre));

            return Ok(query.ToList());
         }


        [HttpPost("AutoPlaceDepotBooks")]
        public async Task<IActionResult> AutoPlaceDepotBooks()
        {
            var depotBooks = await _context.Books
                .Where(b => b.Status == BookStatus.InDepot)
                .ToListAsync();

            var shelves = await _context.Shelves
                .Include(s => s.Books)
                .ToListAsync();

            foreach (var book in depotBooks)
            {
                var shelf = shelves.FirstOrDefault(s =>
                    book.Height <= s.Height &&
                    s.Books.Sum(b => b.Width) + book.Width <= s.Width);

                if (shelf != null)
                {
                    book.ShelfId = shelf.Id;
                    book.Status = BookStatus.OnShelf;
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Auto-placement complete.");
        }

        [HttpGet("Bookshelves")]
        public IActionResult ListBookshelves()
        {
            var shelves = _context.Bookshelves
                .Include(b => b.Shelves)
                .ThenInclude(s => s.Books)
                .ToList();

            var result = shelves.Select(bs => new BookshelfDto
            {
                Label = bs.Label,
                Height = bs.Height,
                Width = bs.Width,
                Shelves = bs.Shelves.Select(sh => new ShelfDto
                {
                    Position = sh.Position,
                    Height = sh.Height,
                    Width = sh.Width,
                    Books = sh.Books.Select(b => new BookDto
                    {
                        Title = b.Title,
                        Width = b.Width,
                        Height = b.Height,
                        Author = b.Author,
                        Genre = b.Genre
                    }).ToList()
                }).ToList()
            });

            return Ok(result);
        }


        [HttpGet("ShelfDetails")]
        public IActionResult GetShelfDetails(string label, int position)
        {
            var shelf = _context.Shelves
                .Include(s => s.Bookshelf)
                .Include(s => s.Books)
                .FirstOrDefault(s => s.Bookshelf.Label == label && s.Position == position);

            if (shelf == null)
                return NotFound("Shelf not found.");

            return Ok(shelf);
        }

        [HttpDelete("RemoveBookshelf")]
        public async Task<IActionResult> RemoveBookshelf(string label)
        {
            var bookshelf = await _context.Bookshelves
                .Include(b => b.Shelves)
                .ThenInclude(s => s.Books)
                .FirstOrDefaultAsync(b => b.Label == label);

            if (bookshelf == null)
                return NotFound("Bookshelf not found.");

            // Move all books on this bookshelf's shelves to depot
            foreach (var shelf in bookshelf.Shelves)
            {
                foreach (var book in shelf.Books)
                {
                    book.ShelfId = null;
                    book.Status = BookStatus.InDepot;
                }
            }

            _context.Bookshelves.Remove(bookshelf);
            await _context.SaveChangesAsync();

            return Ok($"Removed bookshelf {label} and moved all its books to the depot.");
        }


        [HttpPost("AddMultipleBooks")]
        public async Task<IActionResult> AddMultipleBooks([FromBody] List<AddBookRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest("No book data provided.");

            List<Book> addedBooks = new();

            foreach (var request in requests)
            {
                var newBook = new Book
                {
                    Title = request.Title,
                    Author = request.Author,
                    Genre = request.Genre,
                    Height = request.Height,
                    Width = request.Width,
                    Type = request.Type,
                    BorrowerId = string.Empty,
                    Status = BookStatus.InDepot
                };

                if (!string.IsNullOrEmpty(request.Label) && request.Position.HasValue && request.Position.Value > 0)
                {
                    var shelf = await _context.Shelves
                        .Include(s => s.Books)
                        .Include(s => s.Bookshelf)
                        .FirstOrDefaultAsync(s =>
                            s.Position == request.Position &&
                            s.Bookshelf.Label == request.Label);

                    if (shelf != null &&
                        newBook.Height <= shelf.Height &&
                        shelf.Books.Sum(b => b.Width) + newBook.Width <= shelf.Width)
                    {
                        newBook.ShelfId = shelf.Id;
                        newBook.Status = BookStatus.OnShelf;
                    }
                }

                _context.Books.Add(newBook);
                addedBooks.Add(newBook);
            }

            await _context.SaveChangesAsync();
            return Ok(addedBooks);
        }

        [HttpPost("AutoPlaceBook")]
        public async Task<IActionResult> AutoPlaceBook([FromBody] AddBookRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request.");

            var newBook = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Genre = request.Genre,
                Height = request.Height,
                Width = request.Width,
                Type = request.Type,
                BorrowerId = string.Empty,
                Status = BookStatus.InDepot
            };

            // Try to find a shelf that fits
            var shelf = await _context.Shelves
                .Include(s => s.Books)
                .Where(s => s.Height >= newBook.Height)
                .FirstOrDefaultAsync(s => s.Books.Sum(b => b.Width) + newBook.Width <= s.Width);

            if (shelf != null)
            {
                newBook.ShelfId = shelf.Id;
                newBook.Status = BookStatus.OnShelf;
            }

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return Ok(newBook);
        }

        [HttpPost("BorrowBook")]
        public async Task<IActionResult> BorrowBook(string title, string borrowerId)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Title == title && b.Status == BookStatus.OnShelf);

            if (book == null)
                return NotFound("Book not found or not available for borrowing.");

            if (string.IsNullOrWhiteSpace(borrowerId))
                return BadRequest("Borrower ID must be provided.");

            book.Status = BookStatus.Borrowed;
            book.BorrowerId = borrowerId;
            book.ShelfId = null;

            await _context.SaveChangesAsync();
            return Ok($"Book '{book.Title}' borrowed by user {borrowerId}.");
        }

        [HttpPost("ReturnBook")]
        public async Task<IActionResult> ReturnBook(string title, string borrowerId)
        {
            if (string.IsNullOrWhiteSpace(borrowerId))
                return BadRequest("Borrower ID must be provided.");

            var book = await _context.Books
                .FirstOrDefaultAsync(b =>
                    b.Title == title &&
                    b.Status == BookStatus.Borrowed &&
                    b.BorrowerId == borrowerId);

            if (book == null)
                return NotFound("Borrowed book not found for this user.");

            book.Status = BookStatus.InDepot;
            book.BorrowerId = string.Empty;

            await _context.SaveChangesAsync();
            return Ok($"Book '{book.Title}' returned by user {borrowerId}.");
        }


        [HttpGet("ShelfUsage")]
        public IActionResult GetShelfUsage()
        {
            var shelfUsages = _context.Shelves
                .Include(s => s.Books)
                .Include(s => s.Bookshelf)
                .Select(s => new
                {
                    Label = s.Bookshelf.Label,
                    Position = s.Position,
                    UsedWidth = s.Books.Sum(b => b.Width),
                    TotalWidth = s.Width,
                    UsagePercent = s.Width == 0 ? 0 : (int)((double)s.Books.Sum(b => b.Width) / s.Width * 100)
                })
                .ToList();

            return Ok(shelfUsages);
        }

        [HttpGet("ExportBooks")]
        public IActionResult ExportBooks()
        {
            var books = _context.Books
                .Include(b => b.Shelf)
                .ThenInclude(s => s.Bookshelf)
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Title,Author,Genre,Height,Width,Status,BorrowerId,ShelfLabel,ShelfPosition");

            foreach (var book in books)
            {
                var shelfLabel = book.Shelf?.Bookshelf?.Label ?? "";
                var shelfPos = book.Shelf?.Position.ToString() ?? "";

                csv.AppendLine($"{Escape(book.Title)},{Escape(book.Author)},{Escape(book.Genre)},{book.Height},{book.Width},{book.Status},{Escape(book.BorrowerId)},{shelfLabel},{shelfPos}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "books_export.csv");

            string Escape(string? input) => input?.Replace(",", ";").Replace("\n", " ") ?? "";
        }















    }
}
