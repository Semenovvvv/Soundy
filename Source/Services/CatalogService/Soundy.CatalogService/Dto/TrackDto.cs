namespace Soundy.CatalogService.Dto;

public class TrackDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid AlbumId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Duration { get; set; }
    public string? AvatarUrl { get; set; }
}