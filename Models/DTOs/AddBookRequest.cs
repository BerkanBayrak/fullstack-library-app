namespace LibraryApi.Models.DTOs
{
    public class AddBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Width { get; set; }
        public BookType Type { get; set; } = BookType.Other;

        // Matches your models: Bookshelf.Label and Shelf.Position
        public string? Label { get; set; } // Bookshelf label
        public int? Position { get; set; } // Shelf position
    }
}
