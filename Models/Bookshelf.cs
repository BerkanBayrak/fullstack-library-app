using System.Text.Json.Serialization;
namespace LibraryApi.Models
{
    public class Bookshelf
    {
        public int Id { get; set; }

        public string Label { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Shelf> Shelves { get; set; } = new();

        public int Height { get; set; }

        public int Width { get; set; }

    }
}