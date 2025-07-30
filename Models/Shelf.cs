using System.Text.Json.Serialization;
namespace LibraryApi.Models
{
    public class Shelf
    {
        public int Id { get; set; }

        [JsonIgnore]
        public List<Book> Books { get; set; } = new();

        public int Height { get; set; }

        public int Width { get; set; }

        public int BookshelfId { get; set; }
        [JsonIgnore]
        public Bookshelf Bookshelf { get; set; } = null!;

        public int Position { get; set; } 

    }
}