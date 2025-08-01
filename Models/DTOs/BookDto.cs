namespace LibraryApi.Models.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }  // ✅ Add this line

        public string Title { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;

        public string? Label { get; set; } // nullable if in depot
        public int? Position { get; set; }
    }
}
