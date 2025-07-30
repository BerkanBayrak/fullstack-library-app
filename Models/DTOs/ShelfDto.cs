namespace LibraryApi.Models.DTOs
{
    public class ShelfDto
    {
        public int Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public List<BookDto> Books { get; set; } = new();
    }
}
