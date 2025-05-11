using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Dto
{
    public class AlbumDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public int TrackCount { get; set; }
    }
}
