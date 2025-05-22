namespace Soundy.CatalogService.Entities
{
    /// <summary> Трек </summary>
    public class Track
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public User Author { get; set; }
        public Album? Album { get; set; }
        public Guid AlbumId { get; set; }
        public IList<PlaylistTrack> Playlists { get; set; }
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; }
    }
}
