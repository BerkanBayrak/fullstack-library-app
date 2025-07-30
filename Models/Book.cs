using System.Text.Json.Serialization;

namespace LibraryApi.Models
{
    public enum BookType
    {
        Novel,
        Magazine,
        Comic,
        Reference,
        Textbook,
        Other
    }
    public enum BookStatus
    {
        Borrowed,
        OnShelf,
        InDepot
    }
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public int Height { get; set; }

        public int Width { get; set; }

        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;

        public int? ShelfId { get; set; }  
        [JsonIgnore]
        public Shelf? Shelf { get; set; }  


        public BookType Type { get; set; } = BookType.Other;
        public BookStatus Status { get; set; } = BookStatus.InDepot;
        public string BorrowerId { get; set; } = string.Empty;
    }
}