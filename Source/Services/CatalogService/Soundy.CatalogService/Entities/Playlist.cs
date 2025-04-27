namespace Soundy.CatalogService.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public List<Track> Tracks { get; set; }
    }
}
