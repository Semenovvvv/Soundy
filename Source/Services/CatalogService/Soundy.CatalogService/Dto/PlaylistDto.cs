using Soundy.CatalogService.Entities;

namespace Soundy.CatalogService.Dto;

public class PlaylistDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public User Author { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<TrackDto> Tracks { get; set; }
    public int TrackCount { get; set; }
    public string? AvatarUrl { get; set; }
}