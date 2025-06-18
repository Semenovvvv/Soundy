namespace Soundy.CatalogService.Entities
{
    public class Album
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public User? Author { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public IList<Track> Tracks { get; set; }
        public int TrackCount => Tracks.Count();
    }
}
