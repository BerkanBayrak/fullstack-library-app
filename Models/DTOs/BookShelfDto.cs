namespace LibraryApi.Models.DTOs
{
    public class BookshelfDto
    {
        public string Label { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Width { get; set; }
        public List<ShelfDto> Shelves { get; set; } = new();
    }
}
