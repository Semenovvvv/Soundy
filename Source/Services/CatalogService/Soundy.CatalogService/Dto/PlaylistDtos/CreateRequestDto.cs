namespace Soundy.CatalogService.Dto.PlaylistDtos;

public class CreateRequestDto
{
    public string Name { get; set; }
    public Guid AuthorId { get; set; }
}