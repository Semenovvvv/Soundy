namespace Soundy.CatalogService.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public string? AvatarUrl { get; set; }
        //public IList<Track> Tracks { get; set; }
        public IList<PlaylistTrack> Tracks { get; set; }
        public int TrackCount => Tracks.Count();
    }
}
