namespace Soundy.CatalogService.Dto.TrackDto;

public class PlaylistDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}