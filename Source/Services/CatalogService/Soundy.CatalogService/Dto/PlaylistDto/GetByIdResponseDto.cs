namespace Soundy.CatalogService.Dto.PlaylistDto;

public class GetByIdResponseDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Name { get; set; }
    public List<TrackDto> Tracks { get; set; }
}