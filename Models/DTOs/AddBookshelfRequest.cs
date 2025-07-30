namespace LibraryApi.Models.DTOs
{
    public class AddBookshelfRequest
    {
        public string Label { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Width { get; set; }
        public int ShelfHeight { get; set; }  // height of each shelf
    }
}
