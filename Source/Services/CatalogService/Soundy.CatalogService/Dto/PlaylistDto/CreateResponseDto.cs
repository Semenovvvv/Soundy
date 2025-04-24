namespace Soundy.CatalogService.Dto.PlaylistDto;

public class CreateResponseDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Name { get; set; }
}