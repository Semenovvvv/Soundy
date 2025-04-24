namespace Soundy.CatalogService.Dto.TrackDto;

public class TrackDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid UserId { get; set; }
    public Guid PlaylistId { get; set; }
    public int Duration { get; set; }
    public DateTime CreatedAt { get; set; }
}